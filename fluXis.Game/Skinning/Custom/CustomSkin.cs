using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Course;
using fluXis.Game.Skinning.Bases.Judgements;
using fluXis.Game.Skinning.Custom.Health;
using fluXis.Game.Skinning.Custom.HitObjects;
using fluXis.Game.Skinning.Custom.Judgements;
using fluXis.Game.Skinning.Custom.Lighting;
using fluXis.Game.Skinning.Json;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace fluXis.Game.Skinning.Custom;

public class CustomSkin : ISkin
{
    public SkinJson SkinJson { get; }
    private LargeTextureStore textures { get; }
    private Storage storage { get; }
    private ISampleStore samples { get; }

    public CustomSkin(SkinJson skinJson, LargeTextureStore textures, Storage storage, ISampleStore samples)
    {
        SkinJson = skinJson;
        this.textures = textures;
        this.storage = storage;
        this.samples = samples;
    }

    public Texture GetDefaultBackground()
    {
        string path = SkinJson.GetOverrideOrDefault("UserInterface/background") + ".png";
        return storage.Exists(path) ? textures.Get(path) : null;
    }

    public Sample GetUISample(UISamples.SampleType type)
    {
        return type switch
        {
            UISamples.SampleType.Back => samples.Get("Samples/UI/back"),
            UISamples.SampleType.Select => samples.Get("Samples/UI/select"),
            UISamples.SampleType.Hover => samples.Get("Samples/UI/hover"),
            UISamples.SampleType.Click => samples.Get("Samples/UI/click"),
            UISamples.SampleType.ClickDisabled => samples.Get("Samples/UI/click-disabled"),
            _ => null
        };
    }

    public Sample GetCourseSample(CourseScreen.SampleType type)
    {
        return type switch
        {
            CourseScreen.SampleType.Confirm => samples.Get("Samples/Course/confirm"),
            CourseScreen.SampleType.Complete => samples.Get("Samples/Course/complete"),
            CourseScreen.SampleType.Failed => samples.Get("Samples/Course/failed"),
            _ => null
        };
    }

    public Drawable GetStageBackground()
    {
        string path = SkinJson.GetOverrideOrDefault("Stage/background") + ".png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetStageBorder(bool right)
    {
        var path = SkinJson.GetOverrideOrDefault($"Stage/border-{(right ? "right" : "left")}") + ".png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = right ? Anchor.TopRight : Anchor.TopLeft,
                Origin = right ? Anchor.TopLeft : Anchor.TopRight,
                RelativeSizeAxes = Axes.Y,
                Height = 1
            };
        }

        return null;
    }

    public Drawable GetLaneCover(bool bottom)
    {
        var path = SkinJson.GetOverrideOrDefault($"Stage/lane-cover-{(bottom ? "bottom" : "top")}") + ".png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                Origin = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetHealthBarBackground()
    {
        string path = SkinJson.GetOverrideOrDefault("Health/background") + ".png";

        if (storage.Exists(path))
        {
            return new Sprite
            {
                Texture = textures.Get(path)
            };
        }

        return null;
    }

    public Drawable GetHealthBar(HealthProcessor processor)
    {
        string path = SkinJson.GetOverrideOrDefault("Health/foreground") + ".png";
        return storage.Exists(path) ? new SkinnableHealthBar(textures.Get(path)) : null;
    }

    public Drawable GetHitObject(int lane, int keyCount)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/Note/{keyCount}k-{lane}") + ".png";

        if (storage.Exists(path))
        {
            var drawable = new CustomHitObjectPiece(SkinJson, textures.Get(path), keyCount, false);
            drawable.UpdateColor(lane, keyCount);
            return drawable;
        }

        return null;
    }

    public Drawable GetTickNote(int lane, int keyCount)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/Tick/{keyCount}k-{lane}") + ".png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/LongNoteBody/{keyCount}k-{lane}") + ".png";

        if (storage.Exists(path))
        {
            var drawable = new CustomHitObjectBody(SkinJson, keyCount, textures.Get(path));
            drawable.UpdateColor(lane, keyCount);
            return drawable;
        }

        return null;
    }

    public Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/LongNoteEnd/{keyCount}k-{lane}") + ".png";

        if (storage.Exists(path))
        {
            var drawable = new CustomHitObjectPiece(SkinJson, textures.Get(path), keyCount, true);
            drawable.UpdateColor(lane, keyCount);
            return drawable;
        }

        return null;
    }

    public VisibilityContainer GetColumnLighting(int lane, int keyCount)
    {
        string path = SkinJson.GetOverrideOrDefault("Lighting/column-lighting") + ".png";

        if (storage.Exists(path))
        {
            var lighting = new SkinnableHitLighting(textures.Get(path));
            lighting.SetColor(SkinJson, lane, keyCount);
            return lighting;
        }

        return null;
    }

    public Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var path = SkinJson.GetOverrideOrDefault($"Receptor/{keyCount}k-{lane}-{(down ? "down" : "up")}") + ".png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetHitLine()
    {
        string path = SkinJson.GetOverrideOrDefault("Stage/hitline") + ".png";

        if (storage.Exists(path))
        {
            return new Sprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            };
        }

        return null;
    }

    public AbstractJudgementText GetJudgement(Judgement judgement, bool isLate)
    {
        var path = SkinJson.GetOverrideOrDefault($"Judgement/{judgement.ToString().ToLower()}") + ".png";
        var texture = storage.Exists(path) ? textures.Get(path) : null;

        if (texture != null)
            return new SkinnableJudgementText(texture, judgement, isLate);

        return null;
    }

    public Sample GetHitSample() => samples.Get("Samples/Gameplay/hit");

    public Sample[] GetMissSamples()
    {
        if (!storage.ExistsDirectory("Samples/Gameplay")) return null;

        var missSamples = storage.GetFiles("Samples/Gameplay", "miss*")
                                 .Select(file => samples.Get($"{file}"))
                                 .Where(sample => sample != null).ToList();

        return missSamples.Count == 0 ? null : missSamples.ToArray();
    }

    public Sample GetFailSample() => samples.Get("Samples/Gameplay/fail");
    public Sample GetRestartSample() => samples.Get("Samples/Gameplay/restart");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        textures?.Dispose();
        samples?.Dispose();
    }
}
