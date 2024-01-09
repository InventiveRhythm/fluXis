using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Skinning.Default;
using fluXis.Game.Skinning.Json;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Skinning;

public partial class SkinManager : Component, ISkin
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private TextureStore textures { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [Resolved]
    private ISampleStore samples { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    private DefaultSkin defaultSkin { get; set; }
    private ISkin currentSkin { get; set; }

    private const string default_skin_name = "Default";
    private const string default_bright_skin_name = "Default Bright";

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
            default_skin_name,
            default_bright_skin_name
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

        var json = JsonConvert.SerializeObject(SkinJson, Formatting.Indented);
        var path = skinStorage.GetFullPath($"{SkinFolder}/skin.json");
        File.WriteAllText(path, json);
    }

    public void OpenFolder() => PathUtils.OpenFolder(isDefault(SkinFolder) ? skinStorage.GetFullPath(".") : skinStorage.GetFullPath(SkinFolder));

    public void ExportCurrent()
    {
        if (isDefault(SkinFolder)) return;

        var exportPath = host.Storage.GetFullPath("export", true);
        var zipPath = Path.Combine(exportPath, $"{SkinFolder}.fsk");

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
        PathUtils.ShowFile(zipPath);
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

            case default_bright_skin_name:
                return new DefaultBrightSkin(textures, samples);
        }

        var skinJson = new SkinJson();

        try
        {
            if (skinStorage.Exists($"{folder}/skin.json"))
            {
                var path = skinStorage.GetFullPath($"{folder}/skin.json");
                var json = File.ReadAllText(path);
                skinJson = JsonConvert.DeserializeObject<SkinJson>(json);
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

    private bool isDefault(string skinName) => string.Equals(skinName, default_skin_name, StringComparison.CurrentCultureIgnoreCase) || string.Equals(skinName, default_bright_skin_name, StringComparison.CurrentCultureIgnoreCase);

    public Texture GetDefaultBackground() => currentSkin.GetDefaultBackground() ?? defaultSkin.GetDefaultBackground();
    public Drawable GetStageBackground() => currentSkin.GetStageBackground() ?? defaultSkin.GetStageBackground();
    public Drawable GetStageBorder(bool right) => currentSkin.GetStageBorder(right) ?? defaultSkin.GetStageBorder(right);
    public Drawable GetLaneCover(bool bottom) => currentSkin.GetLaneCover(bottom) ?? defaultSkin.GetLaneCover(bottom);

    public Drawable GetHealthBarBackground() => currentSkin.GetHealthBarBackground() ?? defaultSkin.GetHealthBarBackground();
    public Drawable GetHealthBar(HealthProcessor processor) => currentSkin.GetHealthBar(processor) ?? defaultSkin.GetHealthBar(processor);

    public Drawable GetHitObject(int lane, int keyCount) => currentSkin.GetHitObject(lane, keyCount) ?? defaultSkin.GetHitObject(lane, keyCount);
    public Drawable GetLongNoteBody(int lane, int keyCount) => currentSkin.GetLongNoteBody(lane, keyCount) ?? defaultSkin.GetLongNoteBody(lane, keyCount);
    public Drawable GetLongNoteEnd(int lane, int keyCount) => currentSkin.GetLongNoteEnd(lane, keyCount) ?? defaultSkin.GetLongNoteEnd(lane, keyCount);
    public Drawable GetColumnLighting(int lane, int keyCount) => currentSkin.GetColumnLighting(lane, keyCount) ?? defaultSkin.GetColumnLighting(lane, keyCount);
    public Drawable GetReceptor(int lane, int keyCount, bool down) => currentSkin.GetReceptor(lane, keyCount, down) ?? defaultSkin.GetReceptor(lane, keyCount, down);
    public Drawable GetHitLine() => currentSkin.GetHitLine() ?? defaultSkin.GetHitLine();
    public Drawable GetJudgement(Judgement judgement) => currentSkin.GetJudgement(judgement) ?? defaultSkin.GetJudgement(judgement);

    public Sample GetHitSample() => currentSkin.GetHitSample() ?? defaultSkin.GetHitSample();
    public Sample[] GetMissSamples() => currentSkin.GetMissSamples() ?? defaultSkin.GetMissSamples();
    public Sample GetFailSample() => currentSkin.GetFailSample() ?? defaultSkin.GetFailSample();
    public Sample GetRestartSample() => currentSkin.GetRestartSample() ?? defaultSkin.GetRestartSample();
}
