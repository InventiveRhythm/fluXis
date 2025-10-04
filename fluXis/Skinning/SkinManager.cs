using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Integration;
using fluXis.Overlay.Notifications;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Course;
using fluXis.Screens.Edit;
using fluXis.Skinning.Bases.Judgements;
using fluXis.Skinning.Custom;
using fluXis.Skinning.Default;
using fluXis.Skinning.DefaultCircle;
using fluXis.Skinning.Json;
using fluXis.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Skinning;

public partial class SkinManager : Component, ISkin, IDragDropHandler
{
    public string[] AllowedExtensions => new[] { ".fsk" };

    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private TextureStore textures { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [Resolved]
    private ISampleStore samples { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ISteamManager steam { get; set; }

    private DefaultSkin defaultSkin { get; set; }
    private ISkin currentSkin { get; set; }

    public const string DEFAULT_SKIN_NAME = "Default";
    public const string DEFAULT_CIRCLE_SKIN_NAME = "Default Circle";

    public Action SkinChanged { get; set; }
    public Action SkinListChanged { get; set; }

    public SkinJson SkinJson => currentSkin.SkinJson;
    public SkinInfo SkinInfo => currentSkin.SkinJson.Info;
    public string CurrentSkinID { get; private set; } = DEFAULT_SKIN_NAME;
    public string FullSkinPath => getDirectoryForSkin(CurrentSkinID);

    public bool CanChangeSkin { get; set; } = true;

    public bool IsDefault => isDefault(CurrentSkinID);

    private readonly List<SkinInfo> skins = new();
    public IReadOnlyList<SkinInfo> AvailableSkins => skins;

    private Bindable<string> skinName;
    private Storage skinStorage;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        currentSkin = defaultSkin = new DefaultSkin(textures, samples);
        skinStorage = storage.GetStorageForDirectory("skins");
        skinName = config.GetBindable<string>(FluXisSetting.SkinName);

        game.AddDragDropHandler(this);
        ReloadSkinList();
    }

    public void ReloadSkinList()
    {
        var dirs = skinStorage.GetDirectories("").ToArray();
        skins.Clear();

        var def = defaultSkin.SkinJson.Info;
        def.IconTexture = defaultSkin.GetIcon();
        skins.Add(def);

        skins.Add(new SkinInfo
        {
            Name = DEFAULT_CIRCLE_SKIN_NAME,
            Creator = "flustix",
            Path = DEFAULT_CIRCLE_SKIN_NAME,
            IconTexture = textures.Get("Skins/circle.png")
        });

        var extra = new List<SkinInfo>();

        foreach (var dir in dirs)
        {
            if (isDefault(dir))
                continue;

            SkinInfo info = null;

            var path = skinStorage.GetFullPath($"{dir}/skin.json");

            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path).Deserialize<SkinJson>();
                    var skin = createCustomSkin(json, dir, false);
                    info = json.Info;
                    info.IconTexture = skin.GetIcon();
                }
                catch (Exception ex)
                {
                    logParseException(dir, ex);
                }
            }

            info ??= new SkinInfo();
            info.Path = dir;

            if (string.IsNullOrWhiteSpace(info.Name))
                info.Name = info.Path;

            info.Name = info.Name.Trim();
            info.Creator = info.Creator.Trim();
            extra.Add(info);
        }

        var items = steam?.WorkshopItems;

        if (items != null)
        {
            foreach (var item in items)
            {
                var folder = steam.GetWorkshopItemDirectory(item);

                if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
                    continue;

                var id = $"steam/{item}";
                var path = Path.Combine(folder, "skin.json");
                SkinInfo info = null;

                if (File.Exists(path))
                {
                    try
                    {
                        var json = File.ReadAllText(path).Deserialize<SkinJson>();
                        var skin = createCustomSkin(json, folder, true);
                        info = json.Info;
                        info.IconTexture = skin.GetIcon();
                    }
                    catch (Exception ex)
                    {
                        logParseException(id, ex);
                    }
                }
                else
                {
                    info = new SkinInfo
                    {
                        Name = item.ToString(),
                        Creator = "Steam Workshop"
                    };
                }

                info ??= new SkinInfo();
                info.Path = id;
                info.SteamWorkshop = true;
                extra.Add(info);
            }
        }

        extra.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
        skins.AddRange(extra);

        SkinListChanged?.Invoke();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        skinName.BindValueChanged(e =>
        {
            if (!CanChangeSkin)
            {
                skinName.Value = e.OldValue;
                return;
            }

            if (currentSkin is not DefaultSkin)
                currentSkin.Dispose();

            CurrentSkinID = e.NewValue;
            currentSkin = loadSkin(CurrentSkinID);

            SkinChanged?.Invoke();
        }, true);
    }

    public void UploadToWorkshop()
    {
        if (steam is null) return;
        if (IsDefault) return;

        var path = FullSkinPath;
        var idPath = Path.Combine(path, "workshopid.txt");
        ulong? id = null;

        if (File.Exists(idPath))
            id = ulong.Parse(File.ReadAllText(idPath));

        var item = new BasicWorkshopItem(SkinInfo.Name, Path.Combine(path, "icon.png"), path);

        var panel = new EditorUploadOverlay
        {
            Text = id is null ? "Uploading skin..." : "Updating skin...",
        };

        if (panels is not null)
            panels.Content = panel;

        steam.ItemUpdated += itemSubmit;

        if (id is not null)
        {
            panel.SubText = "Updating item...";
            steam.UpdateItem(id.Value, item);
        }
        else
        {
            steam.ItemCreated += itemCreate;
            panel.SubText = "Creating item...";
            steam.UploadItem(item);
        }

        return;

        void hidePanel()
        {
            if (steam is null)
                return;

            steam.ItemUpdated -= itemSubmit;
            steam.ItemCreated -= itemCreate;
            panel.Hide();
        }

        void itemCreate(bool result)
        {
            if (!result)
            {
                hidePanel();
                notifications.SendError("Failed to create item.");
                return;
            }

            panel.SubText = "Uploading...";
        }

        void itemSubmit(bool result)
        {
            hidePanel();

            if (result)
                return;

            notifications.SendError("Failed to submit item.");
        }
    }

    public void SetSkin(SkinInfo info) => skinName.Value = info.Path;

    public void UpdateAndSave(SkinJson newSkinJson)
    {
        if (CurrentSkinID.StartsWith("steam/"))
            return;

        currentSkin = createCustomSkin(newSkinJson, CurrentSkinID, false);

        var json = SkinJson.Serialize(true);
        var path = skinStorage.GetFullPath($"{CurrentSkinID}/skin.json");
        File.WriteAllText(path, json);
    }

    public bool OnDragDrop(string file)
    {
        if (!File.Exists(file))
            return false;

        var zip = new ZipArchive(File.OpenRead(file), ZipArchiveMode.Read);

        var folder = Path.GetFileNameWithoutExtension(file);
        var path = skinStorage.GetFullPath(folder);

        if (Directory.Exists(path))
        {
            notifications.SendError("A skin with this name already exists", "Either delete the existing one, or rename the skin file.");
            return true;
        }

        Directory.CreateDirectory(path);

        foreach (var entry in zip.Entries)
        {
            if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
                continue;

            var filePath = Path.Combine(path, entry.FullName);
            var dir = Path.GetDirectoryName(filePath);

            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            entry.ExtractToFile(filePath, true);
        }

        zip.Dispose();
        File.Delete(file);

        Schedule(() =>
        {
            ReloadSkinList();
            skinName.Value = folder;
        });

        return true;
    }

    public void OpenFolder()
    {
        if (isDefault(CurrentSkinID))
            skinStorage.PresentExternally();
        else
        {
            var path = getDirectoryForSkin(CurrentSkinID);
            host.PresentFileExternally(path);
        }
    }

    public void ExportCurrent()
    {
        if (isDefault(CurrentSkinID)) return;

        var zipPath = game.ExportStorage.GetFullPath($"{CurrentSkinID}.fsk");

        if (File.Exists(zipPath))
            File.Delete(zipPath);

        using var zip = new ZipArchive(File.Create(zipPath), ZipArchiveMode.Create);
        var files = Directory.GetFiles(skinStorage.GetFullPath(CurrentSkinID), "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relativePath = file.Replace(skinStorage.GetFullPath(CurrentSkinID), "");
            zip.CreateEntryFromFile(file, relativePath[1..]);
        }

        game.ExportStorage.PresentFileExternally(zipPath);
    }

    public void Delete(string folder)
    {
        if (isDefault(folder)) return;

        var path = skinStorage.GetFullPath(folder);
        Directory.Delete(path, true);

        var current = CurrentSkinID;
        ReloadSkinList();

        if (folder == current)
            skinName.Value = DEFAULT_SKIN_NAME;
    }

    private ISkin createCustomSkin(SkinJson skinJson, string folder, bool absolute)
    {
        var storage = absolute ? new NativeStorage(folder) : skinStorage.GetStorageForDirectory(folder);
        var resources = new StorageBackedResourceStore(storage);
        var textureStore = new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(resources));
        var sampleStore = audio.GetSampleStore(resources);
        sampleStore.AddExtension("mp3");
        sampleStore.AddExtension("ogg");
        sampleStore.AddExtension("wav");

        return new CustomSkin(skinJson, textureStore, storage, sampleStore);
    }

    private ISkin loadSkin(string id)
    {
        switch (id)
        {
            case DEFAULT_SKIN_NAME:
                return defaultSkin;

            case DEFAULT_CIRCLE_SKIN_NAME:
                return new DefaultCircleSkin(textures, samples);
        }

        var skinJson = new SkinJson();
        var path = getDirectoryForSkin(id);

        try
        {
            var jsonPath = Path.Combine(path, "skin.json");

            if (File.Exists(jsonPath))
                skinJson = File.ReadAllText(jsonPath).Deserialize<SkinJson>();
            else
                Logger.Log($"No skin.json in folder '{id}' found, using default skin.json", LoggingTarget.Information);
        }
        catch (Exception ex)
        {
            logParseException(id, ex);
        }

        skinJson.Info.Path = id;
        return createCustomSkin(skinJson, path, id.StartsWith("steam/"));
    }

    private void logParseException(string folder, Exception ex)
    {
        if (ex is JsonReaderException)
            Logger.Log($"Failed to parse skin.json for '{folder}'! {ex.Message}", LoggingTarget.Runtime, LogLevel.Error);
        else
            Logger.Error(ex, $"Failed to load skin.json '{folder}'");
    }

    private static bool isDefault(string name)
    {
        return string.Equals(name, DEFAULT_SKIN_NAME, StringComparison.CurrentCultureIgnoreCase)
               || string.Equals(name, DEFAULT_CIRCLE_SKIN_NAME, StringComparison.CurrentCultureIgnoreCase);
    }

    private string getDirectoryForSkin(string id)
    {
        if (id.StartsWith("steam/"))
        {
            var idString = id.Split('/')[1];
            if (steam is null || !ulong.TryParse(idString, out var steamId))
                return null;

            var folder = steam.GetWorkshopItemDirectory(steamId);
            return folder;
        }

        return skinStorage.GetFullPath(id);
    }

    #region ISkin Implementation

    Texture ISkin.GetIcon() => currentSkin.GetIcon() ?? defaultSkin.GetIcon();
    Texture ISkin.GetDefaultBackground() => currentSkin.GetDefaultBackground() ?? defaultSkin.GetDefaultBackground();

    Sample ISkin.GetUISample(UISamples.SampleType type) => currentSkin.GetUISample(type) ?? defaultSkin.GetUISample(type);
    Sample ISkin.GetCourseSample(CourseScreen.SampleType type) => currentSkin.GetCourseSample(type) ?? defaultSkin.GetCourseSample(type);

    Drawable ISkin.GetStageBackgroundPart(Anchor part) => currentSkin.GetStageBackgroundPart(part) ?? defaultSkin.GetStageBackgroundPart(part);
    Drawable ISkin.GetLaneCover(bool bottom) => currentSkin.GetLaneCover(bottom) ?? defaultSkin.GetLaneCover(bottom);

    Drawable ISkin.GetHealthBarBackground() => currentSkin.GetHealthBarBackground() ?? defaultSkin.GetHealthBarBackground();
    Drawable ISkin.GetHealthBar(HealthProcessor processor) => currentSkin.GetHealthBar(processor) ?? defaultSkin.GetHealthBar(processor);

    Drawable ISkin.GetHitObject(int lane, int keyCount) => currentSkin.GetHitObject(lane, keyCount) ?? defaultSkin.GetHitObject(lane, keyCount);
    Drawable ISkin.GetTickNote(int lane, int keyCount, bool small) => currentSkin.GetTickNote(lane, keyCount, small) ?? defaultSkin.GetTickNote(lane, keyCount, small);
    Drawable ISkin.GetLandmine(int lane, int keyCount) => currentSkin.GetLandmine(lane, keyCount) ?? defaultSkin.GetLandmine(lane, keyCount);
    Drawable ISkin.GetLongNoteBody(int lane, int keyCount) => currentSkin.GetLongNoteBody(lane, keyCount) ?? defaultSkin.GetLongNoteBody(lane, keyCount);
    Drawable ISkin.GetLongNoteEnd(int lane, int keyCount) => currentSkin.GetLongNoteEnd(lane, keyCount) ?? defaultSkin.GetLongNoteEnd(lane, keyCount);
    VisibilityContainer ISkin.GetColumnLighting(int lane, int keyCount) => currentSkin.GetColumnLighting(lane, keyCount) ?? defaultSkin.GetColumnLighting(lane, keyCount);
    Drawable ISkin.GetReceptor(int lane, int keyCount, bool down) => currentSkin.GetReceptor(lane, keyCount, down) ?? defaultSkin.GetReceptor(lane, keyCount, down);
    Drawable ISkin.GetHitLine() => currentSkin.GetHitLine() ?? defaultSkin.GetHitLine();
    AbstractJudgementText ISkin.GetJudgement(Judgement judgement, bool isLate) => currentSkin.GetJudgement(judgement, isLate) ?? defaultSkin.GetJudgement(judgement, isLate);

    Drawable ISkin.GetFailFlash() => currentSkin.GetFailFlash() ?? defaultSkin.GetFailFlash();

    Drawable ISkin.GetResultsScoreRank(ScoreRank rank) => currentSkin.GetResultsScoreRank(rank) ?? defaultSkin.GetResultsScoreRank(rank);

    Sample ISkin.GetHitSample() => currentSkin.GetHitSample() ?? defaultSkin.GetHitSample();
    Sample[] ISkin.GetMissSamples() => currentSkin.GetMissSamples() ?? defaultSkin.GetMissSamples();
    Sample ISkin.GetFailSample() => currentSkin.GetFailSample() ?? defaultSkin.GetFailSample();
    Sample ISkin.GetRestartSample() => currentSkin.GetRestartSample() ?? defaultSkin.GetRestartSample();
    Sample ISkin.GetFullComboSample() => currentSkin.GetFullComboSample() ?? defaultSkin.GetFullComboSample();
    Sample ISkin.GetAllFlawlessSample() => currentSkin.GetAllFlawlessSample() ?? defaultSkin.GetAllFlawlessSample();

    #endregion
}
