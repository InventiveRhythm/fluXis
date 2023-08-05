using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning.Default.HitObject;
using fluXis.Game.Skinning.Default.Lighting;
using fluXis.Game.Skinning.Default.Receptor;
using fluXis.Game.Skinning.Default.Stage;
using fluXis.Game.Skinning.Json;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Skinning;

public partial class SkinManager : Component
{
    [Resolved]
    private FluXisConfig config { get; set; }

    public Skin CurrentSkin { get; private set; }
    public string SkinFolder { get; private set; } = "Default";
    public bool CanChangeSkin { get; set; } = true;

    private Bindable<string> skinName;

    private LargeTextureStore skinTextures;
    private Storage skinStorage;

    public SkinManager()
    {
        CurrentSkin = new Skin();
    }

    public IEnumerable<string> GetSkinNames()
    {
        string[] skinNames = { "Default" };
        skinNames = skinNames.Concat(skinStorage.GetDirectories("")).ToArray();
        return skinNames;
    }

    [BackgroundDependencyLoader]
    private void load(GameHost host, Storage storage)
    {
        skinStorage = storage.GetStorageForDirectory("skins");
        skinTextures = new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(new StorageBackedResourceStore(skinStorage)));
        skinName = config.GetBindable<string>(FluXisSetting.SkinName);
    }

    public void UpdateAndSave(Skin newSkin)
    {
        CurrentSkin = newSkin;

        var json = JsonConvert.SerializeObject(CurrentSkin, Formatting.Indented);
        var path = $"{SkinFolder}/skin.json";
        skinStorage.Delete(path);
        var stream = skinStorage.GetStream(path, FileAccess.Write);
        var writer = new StreamWriter(stream);
        writer.Write(json);
        writer.Flush();
        stream.Flush();
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

    private void loadConfig()
    {
        try
        {
            if (skinStorage.Exists($"{SkinFolder}/skin.json"))
            {
                var stream = skinStorage.GetStream($"{SkinFolder}/skin.json");
                var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                CurrentSkin = JsonConvert.DeserializeObject<Skin>(json);
                Logger.Log("Loaded skin.json", LoggingTarget.Information);
            }
            else
            {
                Logger.Log("No skin.json found, using default skin", LoggingTarget.Information);
                CurrentSkin = new Skin();
            }
        }
        catch (Exception e)
        {
            CurrentSkin = new Skin();
            Logger.Error(e, "Failed to load skin");
        }
    }

    public Drawable GetStageBorder(bool rightSide)
    {
        var path = $"{SkinFolder}/Stage/border-{(rightSide ? "right" : "left")}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = rightSide ? Anchor.TopRight : Anchor.TopLeft,
                Origin = rightSide ? Anchor.TopLeft : Anchor.TopRight,
                RelativeSizeAxes = Axes.Y,
                Height = 1
            };
        }

        return rightSide ? new DefaultStageBorderRight() : new DefaultStageBorderLeft();
    }

    public Drawable GetStageBackground()
    {
        var path = $"{SkinFolder}/Stage/background.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return new DefaultStageBackground();
    }

    public Drawable GetHitObject(int lane, int maxLanes)
    {
        var path = $"{SkinFolder}/HitObjects/Note/{maxLanes}k-{lane}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        var piece = new DefaultHitObjectPiece();
        piece.UpdateColor(lane, maxLanes);
        return piece;
    }

    public Drawable GetLongNoteBody(int lane, int maxLanes)
    {
        var path = $"{SkinFolder}/HitObjects/LongNoteBody/{maxLanes}k-{lane}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new Sprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        var body = new DefaultHitObjectBody();
        body.UpdateColor(lane, maxLanes);
        return body;
    }

    public Drawable GetLongNoteEnd(int lane, int maxLanes)
    {
        var path = $"{SkinFolder}/HitObjects/LongNoteEnd/{maxLanes}k-{lane}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        var end = new DefaultHitObjectEnd();
        end.UpdateColor(lane, maxLanes);
        return end;
    }

    public Drawable GetColumLighing(int lane, int maxLanes)
    {
        var path = $"{SkinFolder}/Lighting/column-lighting.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        var lighting = new DefaultColumnLighing();
        lighting.UpdateColor(lane, maxLanes);
        return lighting;
    }

    public Drawable GetReceptor(int lane, int maxLanes, bool down)
    {
        var path = $"{SkinFolder}/Receptor/{maxLanes}k-{lane}-{(down ? "down" : "up")}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        var receptor = down ? new DefaultReceptorDown() : new DefaultReceptorUp();
        receptor.UpdateColor(lane, maxLanes);
        receptor.Height = CurrentSkin.GetKeymode(maxLanes).HitPosition;
        return receptor;
    }

    public Drawable GetHitLine()
    {
        var path = $"{SkinFolder}/Stage/hitline.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new Sprite
            {
                Texture = skinTextures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            };
        }

        return new DefaultHitLine();
    }

    public Drawable GetLaneCover(bool bottom)
    {
        var path = $"{SkinFolder}/Stage/lane-cover-{(bottom ? "bottom" : "top")}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new SkinnableSprite
            {
                Texture = skinTextures.Get(path),
                Anchor = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                Origin = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return bottom ? new DefaultBottomLaneCover() : new DefaultTopLaneCover();
    }

    public Drawable GetJudgement(Judgement judgement)
    {
        var path = $"{SkinFolder}/Judgement/{judgement.ToString().ToLower()}.png";

        if (skinStorage?.Exists(path) ?? false)
        {
            return new Sprite
            {
                Texture = skinTextures.Get(path)
            };
        }

        return null;
    }
}
