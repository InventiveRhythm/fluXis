using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Modes;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Import;

public class FluXisImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".fms" };

    /**
     * Used to set the status of the next imported mapset.
     */
    public int MapStatus { get; set; } = -2;

    public Action<float> OnProgress { get; set; }
    public Action<bool> OnComplete { get; set; }

    public TaskNotificationData Notification { get; set; }
    public GameModeManager GameModes { get; set; }

    public override void Import(string path)
    {
        if (!File.Exists(path))
        {
            OnComplete?.Invoke(false);
            return;
        }

        if (Notification == null)
        {
            Notification = new TaskNotificationData
            {
                Text = "Importing Mapset...",
                TextWorking = "Processing...",
                TextFinished = "Done! Click to view."
            };

            Notifications.AddTask(Notification);
        }

        Notification.State = LoadingState.UnknownProgress;

        try
        {
            var set = getRealmMaps(path);

            var existing = Realm.Run(r =>
            {
                var sets = r.All<RealmMapSet>();
                return sets.FirstOrDefault(x => x.OnlineID == set.OnlineID)?.Detach();
            });

            if (existing != null && existing.OnlineID > 0)
            {
                var updatedPath = MapFiles.GetFullPath(set.ID.ToString()) + "/";
                var originalPath = MapFiles.GetFullPath(existing.ID.ToString()) + "/";
                var files = Directory.GetFiles(updatedPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var f = new FileInfo(file);
                    f.MoveTo($"{originalPath}/{f.Name}", true);
                }

                Directory.Delete(updatedPath);

                importAsUpdate(set, existing);
            }
            else
            {
                if (set.Maps.Count > 0)
                {
                    Realm.RunWrite(realm =>
                    {
                        realm.Add(set);

                        set = set.Detach();
                        MapStore.AddMapSet(set);
                    });
                }
            }

            try { File.Delete(path); }
            catch { Logger.Log($"Failed to delete {path}"); }

            Notification.ClickAction = () => MapStore.Present(set);
            Notification.State = LoadingState.Complete;
            OnComplete?.Invoke(true);
        }
        catch (Exception e)
        {
            Notification.State = LoadingState.Failed;
            Logger.Error(e, "Failed to import mapset");
            OnComplete?.Invoke(false);
        }
    }

    public void ImportAsUpdate(string path, RealmMapSet original)
    {
        var updated = getRealmMaps(path, original.ID);
        importAsUpdate(updated, original);
    }

    private void importAsUpdate(RealmMapSet updated, RealmMapSet original)
    {
        Realm.RunWrite(r =>
        {
            // replace with realm version
            original = r.Find<RealmMapSet>(original.ID);
            updated.ID = original.ID;

            foreach (var oMap in original.Maps)
            {
                var uMap = updated.Maps.FirstOrDefault(x => x.FileName == oMap.FileName);

                // map got deleted in the new version
                if (uMap == null)
                {
                    original.Maps.Remove(oMap);
                    r.Remove(oMap);
                    continue;
                }

                uMap.ID = oMap.ID;
            }

            foreach (var uMap in updated.Maps)
            {
                var oMap = original.Maps.FirstOrDefault(x => x.FileName == uMap.FileName);

                if (uMap == null)
                {
                    original.Maps.Remove(oMap);
                    r.Remove(oMap);
                }
            }

            updated.CopyChanges(original);

            original = original.Detach();
        });

        MapStore.UpdateMapSet(original, original);
    }

    private RealmMapSet getRealmMaps(string path, Guid? id = null)
    {
        using var archive = ZipFile.OpenRead(path);
        var maps = new List<RealmMap>();

        var set = new RealmMapSet(maps) { ID = id ?? Guid.NewGuid() };
        MapStore.AssignResources(set);

        var dir = MapFiles.GetFullPath(set.ID.ToString()) + "/";

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        foreach (var entry in archive.Entries)
        {
            var filePath = dir + entry.FullName.Replace('\\', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            entry.ExtractToFile(filePath, true);
        }

        var fileCount = archive.Entries.Count;
        var idx = 0;

        Dictionary<string, string> audioHashes = new();
        var storage = new NativeStorage(dir);

        foreach (var file in storage.GetFiles(string.Empty))
        {
            var format = GameModes.GetFormat(storage, Path.GetExtension(file) ?? string.Empty);

            if (format?.IsChart(file) ?? false)
            {
                var playable = format.Parse(file);
                if (playable is null) throw new InvalidOperationException($"Failed to parse chart as playable. [fmt: {format}, file: {file}]");

                if (!audioHashes.TryGetValue(playable.AudioFile, out var audioHash))
                {
                    var audioEntry = archive.GetEntry(playable.AudioFile);
                    audioHash = audioEntry != null ? MapUtils.GetXXHash(audioEntry.ReadAllBytes()) : "";
                    audioHashes[playable.AudioFile] = audioHash;
                }

                var realm = new RealmMap { FileName = file, AudioHash = audioHash, StatusInt = MapStatus, MapSet = set };
                playable.SaveIntoRealmMap(realm);
                maps.Add(realm);

                if (string.IsNullOrEmpty(realm.Metadata.ColorHex))
                {
                    var background = realm.GetBackgroundStream();

                    if (background != null)
                    {
                        var color = ImageUtils.GetAverageColour(background);

                        if (color != Colour4.Transparent)
                            realm.Metadata.Color = color;
                    }
                    else
                        Logger.Log("Failed to load background for color extraction");
                }

                if (!string.IsNullOrEmpty(playable.CoverFile))
                    set.Cover = playable.CoverFile;

                if (realm.StatusInt >= 100)
                    continue;

                try
                {
                    var lookup = MapStore.LookUpHash(realm.Hash);
                    if (lookup == null) continue;

                    realm.OnlineID = lookup.ID;
                    realm.StatusInt = lookup.Status;
                    realm.Rating = (float)lookup.Rating;
                    set.OnlineID = lookup.SetID;
                    set.DateSubmitted = TimeUtils.GetFromSeconds(lookup.DateSubmitted);
                    realm.LastOnlineUpdate = TimeUtils.GetFromSeconds(lookup.LastUpdated);

                    if (lookup.DateRanked != null)
                        set.DateRanked = TimeUtils.GetFromSeconds(lookup.DateRanked.Value);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to look up map online.");
                }
            }

            var prog = (float)++idx / fileCount;
            Notification.Progress = prog;
            OnProgress?.Invoke(prog);
        }

        return set;
    }
}
