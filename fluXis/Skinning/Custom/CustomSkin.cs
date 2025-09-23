using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Course;
using fluXis.Skinning.Bases.Judgements;
using fluXis.Skinning.Custom.Health;
using fluXis.Skinning.Custom.HitObjects;
using fluXis.Skinning.Custom.Judgements;
using fluXis.Skinning.Custom.Lighting;
using fluXis.Skinning.Custom.Receptor;
using fluXis.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Skinning.Custom;

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

    public Texture GetIcon()
    {
        const string path = "icon.png";
        return storage.Exists(path) ? textures.Get(path) : null;
    }

    public Texture GetDefaultBackground()
    {
        string path = SkinJson.GetOverrideOrDefault("UserInterface/background") + ".png";
        return storage.Exists(path) ? textures.Get(path) : null;
    }

    public Sample GetUISample(UISamples.SampleType type) => type switch
    {
        UISamples.SampleType.Back => samples.Get("Samples/UI/back"),
        UISamples.SampleType.Select => samples.Get("Samples/UI/select"),
        UISamples.SampleType.Hover => samples.Get("Samples/UI/hover"),
        UISamples.SampleType.Click => samples.Get("Samples/UI/click"),
        UISamples.SampleType.ClickDisabled => samples.Get("Samples/UI/click-disabled"),
        UISamples.SampleType.SkinSelectClick => samples.Get("Samples/UI/skin-select-click"),
        _ => null
    };

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

    public Drawable GetStageBackgroundPart(Anchor part)
    {
        var file = part switch
        {
            Anchor.TopLeft => "border-left-top",
            Anchor.TopCentre => "background-top",
            Anchor.TopRight => "border-right-top",
            Anchor.CentreLeft => "border-left",
            Anchor.Centre => "background",
            Anchor.CentreRight => "border-right",
            Anchor.BottomLeft => "border-left-bottom",
            Anchor.BottomCentre => "background-bottom",
            Anchor.BottomRight => "border-right-bottom",
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };

        string path = SkinJson.GetOverrideOrDefault($"Stage/{file}") + ".png";

        if (!storage.Exists(path))
            return null;

        var sprite = new SkinnableSprite(textures.Get(path));

        switch (part)
        {
            case Anchor.Centre:
                sprite.RelativeSizeAxes = Axes.Both;
                sprite.Size = Vector2.One;
                break;

            case Anchor.CentreLeft:
            case Anchor.CentreRight:
                sprite.RelativeSizeAxes = Axes.Y;
                sprite.BypassAutoSizeAxes = Axes.None;
                sprite.Height = 1;
                break;

            case Anchor.TopCentre:
            case Anchor.BottomCentre:
                sprite.RelativeSizeAxes = Axes.X;
                sprite.Width = 1;
                sprite.SkipResizing = true;
                break;

            case Anchor.TopLeft:
            case Anchor.TopRight:
            case Anchor.BottomLeft:
            case Anchor.BottomRight:
                sprite.RelativeSizeAxes = Axes.X;
                sprite.Width = 1;
                break;
        }

        return sprite;
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
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/Note/{keyCount}k-{lane}");
        var main = path + ".png";
        var tintless = path + "-tintless.png";

        if (!storage.Exists(main))
            return null;

        var index = Theme.GetLaneColorIndex(lane, keyCount);

        var drawable = new CustomHitObjectPiece(
            SkinJson, (MapColor)index, keyCount, false,
            textures.Get(main), textures.Get(tintless)
        );

        drawable.UpdateColor(lane, keyCount);
        return drawable;
    }

    public Drawable GetTickNote(int lane, int keyCount, bool small)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/Tick/{keyCount}k-{lane}{(small ? "-small" : "")}") + ".png";

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
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/LongNoteBody/{keyCount}k-{lane}");
        var main = path + ".png";
        var tintless = path + "-tintless.png";

        if (!storage.Exists(main))
            return null;

        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var drawable = new CustomHitObjectBody(
            SkinJson, (MapColor)index, keyCount,
            textures.Get(main), textures.Get(tintless)
        );

        drawable.UpdateColor(lane, keyCount);
        return drawable;
    }

    public Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var path = SkinJson.GetOverrideOrDefault($"HitObjects/LongNoteEnd/{keyCount}k-{lane}");
        var main = path + ".png";
        var tintless = path + "-tintless.png";

        if (!storage.Exists(main))
            return null;

        var index = Theme.GetLaneColorIndex(lane, keyCount);

        var drawable = new CustomHitObjectPiece(
            SkinJson, (MapColor)index, keyCount, true,
            textures.Get(main), textures.Get(tintless)
        );

        drawable.UpdateColor(lane, keyCount);
        return drawable;
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
        var path = SkinJson.GetOverrideOrDefault($"Receptor/{keyCount}k-{lane}-{(down ? "down" : "up")}");
        var main = path + ".png";
        var tintless = path + "-tintless.png";

        if (!storage.Exists(main))
            return null;

        var index = Theme.GetLaneColorIndex(lane, keyCount);

        var drawable = new CustomReceptor(
            SkinJson, (MapColor)index, keyCount,
            textures.Get(main), textures.Get(tintless)
        );

        drawable.UpdateColor(lane, keyCount);
        return drawable;
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

    public Drawable GetFailFlash()
    {
        var path = SkinJson.GetOverrideOrDefault("Gameplay/fail-flash") + ".png";
        return storage.Exists(path) ? new SkinnableSprite(textures.Get(path)) { FillMode = FillMode.Fill } : null;
    }

    public Drawable GetResultsScoreRank(ScoreRank rank)
    {
        var path = SkinJson.GetOverrideOrDefault($"Results/rank-{rank.ToString().ToLower()}") + ".png";
        return storage.Exists(path) ? new SkinnableSprite(textures.Get(path)) : null;
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
    public Sample GetFullComboSample() => samples.Get("Samples/Gameplay/full-combo");
    public Sample GetAllFlawlessSample() => samples.Get("Samples/Gameplay/all-flawless");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        textures?.Dispose();
        samples?.Dispose();
    }
}
