using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Course;
using fluXis.Game.Skinning.Bases.Judgements;
using fluXis.Game.Skinning.Custom;
using fluXis.Game.Skinning.Default;
using fluXis.Game.Skinning.Json;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring.Enums;
using fluXis.Shared.Utils;
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

namespace fluXis.Game.Skinning;

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

    private DefaultSkin defaultSkin { get; set; }
    private ISkin currentSkin { get; set; }

    private const string default_skin_name = "Default";

    public Action SkinChanged { get; set; }
    public Action SkinListChanged { get; set; }

    public SkinJson SkinJson => currentSkin.SkinJson;
    public string SkinFolder { get; private set; } = default_skin_name;
    public bool CanChangeSkin { get; set; } = true;

    public bool IsDefault => isDefault(SkinFolder);

    private Bindable<string> skinName;
    private Storage skinStorage;

    public IEnumerable<string> GetSkinNames()
    {
        string[] defaultSkins =
        {
            default_skin_name
        };

        var custom = skinStorage.GetDirectories("").ToArray();

        // remove default skins from customs if they exist
        custom = custom.Where(x => !isDefault(x)).ToArray();

        return defaultSkins.Concat(custom).ToArray();
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        currentSkin = defaultSkin = new DefaultSkin(textures, samples);
        skinStorage = storage.GetStorageForDirectory("skins");
        skinName = config.GetBindable<string>(FluXisSetting.SkinName);

        game.AddDragDropHandler(this);
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

            SkinFolder = e.NewValue;
            currentSkin = loadSkin(SkinFolder);
            Logger.Log($"Switched skin to '{SkinFolder}'", LoggingTarget.Information);

            SkinChanged?.Invoke();
        }, true);
    }

    public void UpdateAndSave(SkinJson newSkinJson)
    {
        currentSkin = createCustomSkin(newSkinJson, SkinFolder);

        var json = SkinJson.Serialize(true);
        var path = skinStorage.GetFullPath($"{SkinFolder}/skin.json");
        File.WriteAllText(path, json);
    }

    public bool OnDragDrop(string file)
    {
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
            var filePath = Path.Combine(path, entry.FullName);

            if (filePath.Contains(Path.DirectorySeparatorChar))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            entry.ExtractToFile(filePath, true);
        }

        Logger.Log($"Imported skin '{folder}'", LoggingTarget.Information);

        zip.Dispose();
        File.Delete(file);

        Schedule(() =>
        {
            SkinListChanged?.Invoke();
            skinName.Value = folder;
        });
        return true;
    }

    public void OpenFolder()
    {
        if (isDefault(SkinFolder))
            skinStorage.PresentExternally();
        else
            skinStorage.PresentFileExternally($"{SkinFolder}/.");
    }

    public void ExportCurrent()
    {
        if (isDefault(SkinFolder)) return;

        var zipPath = game.ExportStorage.GetFullPath($"{SkinFolder}.fsk");

        if (File.Exists(zipPath))
            File.Delete(zipPath);

        using var zip = new ZipArchive(File.Create(zipPath), ZipArchiveMode.Create);
        var files = Directory.GetFiles(skinStorage.GetFullPath(SkinFolder), "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relativePath = file.Replace(skinStorage.GetFullPath(SkinFolder), "");
            zip.CreateEntryFromFile(file, relativePath[1..]);
        }

        Logger.Log($"Exported skin '{SkinFolder}' to '{zipPath}'", LoggingTarget.Information);
        game.ExportStorage.PresentFileExternally(zipPath);
    }

    public void Delete(string folder)
    {
        if (isDefault(folder)) return;

        var path = skinStorage.GetFullPath(folder);
        Directory.Delete(path, true);
        Logger.Log($"Deleted skin '{folder}'", LoggingTarget.Information);

        if (folder == SkinFolder)
            skinName.Value = default_skin_name;

        SkinListChanged?.Invoke();
    }

    private ISkin createCustomSkin(SkinJson skinJson, string folder)
    {
        var storage = skinStorage.GetStorageForDirectory(folder);
        var resources = new StorageBackedResourceStore(storage);
        var textureStore = new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(resources));
        var sampleStore = audio.GetSampleStore(resources);
        sampleStore.AddExtension("mp3");
        sampleStore.AddExtension("ogg");
        sampleStore.AddExtension("wav");

        return new CustomSkin(skinJson, textureStore, storage, sampleStore);
    }

    private ISkin loadSkin(string folder)
    {
        switch (folder)
        {
            case default_skin_name:
                return defaultSkin;
        }

        var skinJson = new SkinJson();

        try
        {
            if (skinStorage.Exists($"{folder}/skin.json"))
            {
                var path = skinStorage.GetFullPath($"{folder}/skin.json");
                skinJson = File.ReadAllText(path).Deserialize<SkinJson>();
                Logger.Log($"Loaded skin.json from '{folder}'", LoggingTarget.Information);
            }
            else
                Logger.Log($"No skin.json in folder '{folder}' found, using default skin.json", LoggingTarget.Information);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load skin.json '{folder}'");
        }

        return createCustomSkin(skinJson, folder);
    }

    private bool isDefault(string skinName) => string.Equals(skinName, default_skin_name, StringComparison.CurrentCultureIgnoreCase);

    public Texture GetDefaultBackground() => currentSkin.GetDefaultBackground() ?? defaultSkin.GetDefaultBackground();

    public Sample GetUISample(UISamples.SampleType type) => currentSkin.GetUISample(type) ?? defaultSkin.GetUISample(type);
    public Sample GetCourseSample(CourseScreen.SampleType type) => currentSkin.GetCourseSample(type) ?? defaultSkin.GetCourseSample(type);

    public Drawable GetStageBackground() => currentSkin.GetStageBackground() ?? defaultSkin.GetStageBackground();
    public Drawable GetStageBorder(bool right) => currentSkin.GetStageBorder(right) ?? defaultSkin.GetStageBorder(right);
    public Drawable GetLaneCover(bool bottom) => currentSkin.GetLaneCover(bottom) ?? defaultSkin.GetLaneCover(bottom);

    public Drawable GetHealthBarBackground() => currentSkin.GetHealthBarBackground() ?? defaultSkin.GetHealthBarBackground();
    public Drawable GetHealthBar(HealthProcessor processor) => currentSkin.GetHealthBar(processor) ?? defaultSkin.GetHealthBar(processor);

    public Drawable GetHitObject(int lane, int keyCount) => currentSkin.GetHitObject(lane, keyCount) ?? defaultSkin.GetHitObject(lane, keyCount);
    public Drawable GetTickNote(int lane, int keyCount) => currentSkin.GetTickNote(lane, keyCount) ?? defaultSkin.GetTickNote(lane, keyCount);
    public Drawable GetLongNoteBody(int lane, int keyCount) => currentSkin.GetLongNoteBody(lane, keyCount) ?? defaultSkin.GetLongNoteBody(lane, keyCount);
    public Drawable GetLongNoteEnd(int lane, int keyCount) => currentSkin.GetLongNoteEnd(lane, keyCount) ?? defaultSkin.GetLongNoteEnd(lane, keyCount);
    public VisibilityContainer GetColumnLighting(int lane, int keyCount) => currentSkin.GetColumnLighting(lane, keyCount) ?? defaultSkin.GetColumnLighting(lane, keyCount);
    public Drawable GetReceptor(int lane, int keyCount, bool down) => currentSkin.GetReceptor(lane, keyCount, down) ?? defaultSkin.GetReceptor(lane, keyCount, down);
    public Drawable GetHitLine() => currentSkin.GetHitLine() ?? defaultSkin.GetHitLine();
    public AbstractJudgementText GetJudgement(Judgement judgement, bool isLate) => currentSkin.GetJudgement(judgement, isLate) ?? defaultSkin.GetJudgement(judgement, isLate);

    public Sample GetHitSample() => currentSkin.GetHitSample() ?? defaultSkin.GetHitSample();
    public Sample[] GetMissSamples() => currentSkin.GetMissSamples() ?? defaultSkin.GetMissSamples();
    public Sample GetFailSample() => currentSkin.GetFailSample() ?? defaultSkin.GetFailSample();
    public Sample GetRestartSample() => currentSkin.GetRestartSample() ?? defaultSkin.GetRestartSample();
}
