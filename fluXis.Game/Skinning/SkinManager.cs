using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Default;
using fluXis.Game.Skinning.Json;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
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
    private GameHost host { get; set; }

    private DefaultSkin defaultSkin { get; set; }
    private ISkin currentSkin { get; set; }

    private const string default_skin_name = "Default";

    public Skin Skin => currentSkin.Skin;
    public string SkinFolder { get; private set; } = default_skin_name;
    public bool CanChangeSkin { get; set; } = true;

    private Bindable<string> skinName;
    private Storage skinStorage;

    public IEnumerable<string> GetSkinNames()
    {
        string[] skinNames = { "Default" };
        skinNames = skinNames.Concat(skinStorage.GetDirectories("")).ToArray();
        return skinNames;
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        currentSkin = defaultSkin = new DefaultSkin(textures);
        skinStorage = storage.GetStorageForDirectory("skins");
        skinName = config.GetBindable<string>(FluXisSetting.SkinName);
    }

    public void UpdateAndSave(Skin newSkin)
    {
        currentSkin = createCustomSkin(newSkin, SkinFolder);

        var json = JsonConvert.SerializeObject(Skin, Formatting.Indented);
        var path = $"{SkinFolder}/skin.json";
        skinStorage.Delete(path);
        var stream = skinStorage.GetStream(path, FileAccess.Write);
        var writer = new StreamWriter(stream);
        writer.Write(json);
        writer.Flush();
        stream.Flush();
    }

    public void OpenFolder()
    {
        PathUtils.OpenFolder(SkinFolder == default_skin_name ? skinStorage.GetFullPath(".") : skinStorage.GetFullPath(SkinFolder));
    }

    protected override void Update()
    {
        if (skinName.Value != SkinFolder) // i dont know why normal bindables dont work
        {
            if (!CanChangeSkin)
            {
                skinName.Value = SkinFolder;
                return;
            }

            SkinFolder = skinName.Value;
            loadConfig();
        }
    }

    private LargeTextureStore createTextureStore(string folder) => new(host.Renderer, host.CreateTextureLoaderStore(new StorageBackedResourceStore(skinStorage.GetStorageForDirectory(folder))));
    private ISkin createCustomSkin(Skin skin, string folder) => new CustomSkin(skin, createTextureStore(folder), skinStorage.GetStorageForDirectory(folder));

    private void loadConfig()
    {
        try
        {
            if (skinStorage.Exists($"{SkinFolder}/skin.json"))
            {
                var stream = skinStorage.GetStream($"{SkinFolder}/skin.json");
                var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                currentSkin = createCustomSkin(JsonConvert.DeserializeObject<Skin>(json), SkinFolder);
                Logger.Log("Loaded skin.json", LoggingTarget.Information);
            }
            else
            {
                Logger.Log("No skin.json found, using default skin", LoggingTarget.Information);
                currentSkin = defaultSkin;
            }
        }
        catch (Exception e)
        {
            currentSkin = defaultSkin;
            Logger.Error(e, "Failed to load skin");
        }
    }

    public Texture GetDefaultBackground() => currentSkin.GetDefaultBackground() ?? defaultSkin.GetDefaultBackground();
    public Drawable GetStageBackground() => currentSkin.GetStageBackground() ?? defaultSkin.GetStageBackground();
    public Drawable GetStageBorder(bool right) => currentSkin.GetStageBorder(right) ?? defaultSkin.GetStageBorder(right);
    public Drawable GetLaneCover(bool bottom) => currentSkin.GetLaneCover(bottom) ?? defaultSkin.GetLaneCover(bottom);
    public Drawable GetHitObject(int lane, int keyCount) => currentSkin.GetHitObject(lane, keyCount) ?? defaultSkin.GetHitObject(lane, keyCount);
    public Drawable GetLongNoteBody(int lane, int keyCount) => currentSkin.GetLongNoteBody(lane, keyCount) ?? defaultSkin.GetLongNoteBody(lane, keyCount);
    public Drawable GetLongNoteEnd(int lane, int keyCount) => currentSkin.GetLongNoteEnd(lane, keyCount) ?? defaultSkin.GetLongNoteEnd(lane, keyCount);
    public Drawable GetColumnLighting(int lane, int keyCount) => currentSkin.GetColumnLighting(lane, keyCount) ?? defaultSkin.GetColumnLighting(lane, keyCount);
    public Drawable GetReceptor(int lane, int keyCount, bool down) => currentSkin.GetReceptor(lane, keyCount, down) ?? defaultSkin.GetReceptor(lane, keyCount, down);
    public Drawable GetHitLine() => currentSkin.GetHitLine() ?? defaultSkin.GetHitLine();
    public Drawable GetJudgement(Judgement judgement) => currentSkin.GetJudgement(judgement) ?? defaultSkin.GetJudgement(judgement);
}
