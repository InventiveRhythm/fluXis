using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Overlay;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens.Menu;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game
{
    public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisKeybind>
    {
        private ScreenStack screenStack;
        private OnlineOverlay overlay;
        private SettingsMenu settings;

        private Storage storage;

        public bool Sex = true;

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            this.storage = storage;

            Discord.Init();

            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Children = new Drawable[]
            {
                new Conductor(),
                BackgroundStack,
                screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both },
                overlay = new OnlineOverlay(),
                settings = new SettingsMenu(),
                new VolumeOverlay()
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Fluxel.Connect();
            screenStack.Push(new MenuScreen());
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);

            Version version = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();

            var window = (SDL2DesktopWindow)host.Window;
            window.Title = "fluXis v" + version;
            window.ConfineMouseMode.Value = ConfineMouseMode.Never;
            // window.CursorState = CursorState.Hidden;
            window.DragDrop += f => onDragDrop(new[] { f });
        }

        private void onDragDrop(string[] paths)
        {
            try
            {
                foreach (var path in paths)
                {
                    if (Path.GetExtension(path) == ".fms")
                    {
                        Logger.Log($"Loading mapset from {path}");

                        ZipArchive archive = ZipFile.OpenRead(path);

                        List<RealmFile> files = new();
                        List<RealmMap> maps = new();

                        RealmMapSet mapSet = new(maps, files);

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            SHA256 sha256 = SHA256.Create();
                            string hash = BitConverter.ToString(sha256.ComputeHash(entry.Open())).Replace("-", "").ToLower();

                            string filename = entry.FullName;

                            files.Add(new RealmFile
                            {
                                Hash = hash,
                                Name = entry.FullName,
                            });

                            if (filename.EndsWith(".fsc"))
                            {
                                string json = new StreamReader(entry.Open()).ReadToEnd();
                                MapInfo mapInfo = JsonConvert.DeserializeObject<MapInfo>(json);

                                float length = 0;
                                int keys = 0;
                                float bpmMin = float.MaxValue;
                                float bpmMax = float.MinValue;

                                foreach (var point in mapInfo.TimingPoints)
                                {
                                    bpmMin = Math.Min(bpmMin, point.BPM);
                                    bpmMax = Math.Max(bpmMax, point.BPM);
                                }

                                foreach (var hitObject in mapInfo.HitObjects)
                                {
                                    float time = hitObject.Time;
                                    if (hitObject.IsLongNote()) time += hitObject.HoldTime;
                                    length = Math.Max(length, time);

                                    keys = Math.Max(keys, hitObject.Lane);
                                }

                                RealmMap map = new RealmMap(new RealmMapMetadata
                                {
                                    Title = mapInfo.Metadata.Title ?? "Untitled",
                                    Artist = mapInfo.Metadata.Artist ?? "Unknown",
                                    Mapper = mapInfo.Metadata.Mapper ?? "Unknown",
                                    Source = mapInfo.Metadata.Source ?? "",
                                    Tags = mapInfo.Metadata.Tags ?? "",
                                    Audio = mapInfo.AudioFile,
                                    Background = mapInfo.BackgroundFile,
                                    PreviewTime = mapInfo.Metadata.PreviewTime,
                                })
                                {
                                    Difficulty = mapInfo.Metadata.Difficulty ?? "Unknown",
                                    MapSet = mapSet,
                                    Hash = hash,
                                    KeyCount = keys,
                                    Length = length,
                                    BPMMin = bpmMin,
                                    BPMMax = bpmMax,
                                    Rating = 0,
                                };

                                maps.Add(map);
                            }
                        }

                        if (files.Count > 0 && maps.Count > 0)
                        {
                            Realm.RunWrite(realm =>
                            {
                                realm.Add(mapSet);
                                MapStore.AddMapSet(mapSet.Detach());

                                foreach (var file in files)
                                {
                                    string filePath = storage.GetStorageForDirectory("files").GetFullPath(file.GetPath());
                                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                                    if (File.Exists(filePath)) continue;

                                    ZipArchiveEntry entry = archive.GetEntry(file.Name);
                                    entry.ExtractToFile(filePath);
                                }

                                archive.Dispose();

                                try { File.Delete(path); }
                                catch { Logger.Log($"Failed to delete {path}"); }
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while importing mapset");
            }
        }

        public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
        {
            switch (e.Action)
            {
                case FluXisKeybind.ToggleOnlineOverlay:
                    overlay.ToggleVisibility();
                    return true;

                case FluXisKeybind.ToggleSettings:
                    settings.ToggleVisibility();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
    }
}
