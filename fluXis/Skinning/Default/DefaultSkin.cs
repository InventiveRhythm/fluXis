using System;
using fluXis.Audio;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Course;
using fluXis.Skinning.Bases.Judgements;
using fluXis.Skinning.Default.Health;
using fluXis.Skinning.Default.HitObject;
using fluXis.Skinning.Default.Judgements;
using fluXis.Skinning.Default.Lighting;
using fluXis.Skinning.Default.Receptor;
using fluXis.Skinning.Default.Results;
using fluXis.Skinning.Default.Stage;
using fluXis.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Default;

public class DefaultSkin : ISkin
{
    public SkinJson SkinJson { get; }
    protected TextureStore Textures { get; }
    private ISampleStore samples { get; }

    public const float BORDER_BASE = 8;
    public const float BORDER_COLOR = 4;

    public DefaultSkin(TextureStore textures, ISampleStore samples)
    {
        SkinJson = CreateJson();
        Textures = textures;
        this.samples = samples;
    }

    protected virtual SkinJson CreateJson() => new DefaultSkinJson
    {
        Info = new SkinInfo
        {
            Name = SkinManager.DEFAULT_SKIN_NAME,
            Creator = "flustix",
            Path = SkinManager.DEFAULT_SKIN_NAME
        }
    };

    public virtual Texture GetIcon() => Textures.Get("Skins/default.png");
    public Texture GetDefaultBackground() => Textures.Get("Backgrounds/default.png");

    public Sample GetUISample(UISamples.SampleType type)
    {
        return type switch
        {
            UISamples.SampleType.Back => samples.Get("UI/back"),
            UISamples.SampleType.Select => samples.Get("UI/accept"),
            UISamples.SampleType.Hover => samples.Get("UI/hover"),
            UISamples.SampleType.Click => samples.Get("UI/click"),
            UISamples.SampleType.ClickDisabled => samples.Get("UI/click-disabled"),
            _ => null
        };
    }

    public Sample GetCourseSample(CourseScreen.SampleType type)
    {
        return null;
    }

    private const float extended = 48;

    public virtual Drawable GetStageBackgroundPart(Anchor part) => part switch
    {
        Anchor.TopLeft => new DefaultStageBorderLeft(SkinJson)
        {
            RelativeSizeAxes = Axes.None,
            Colour = ColourInfo.GradientVertical(Colour4.White.Opacity(0), Colour4.White),
            Height = extended
        },
        Anchor.TopCentre => new DefaultStageBackgroundTop { Height = extended },
        Anchor.TopRight => new DefaultStageBorderRight(SkinJson)
        {
            RelativeSizeAxes = Axes.None,
            Colour = ColourInfo.GradientVertical(Colour4.White.Opacity(0), Colour4.White),
            Height = extended
        },
        Anchor.CentreLeft => new DefaultStageBorderLeft(SkinJson),
        Anchor.Centre => new DefaultStageBackground(),
        Anchor.CentreRight => new DefaultStageBorderRight(SkinJson),
        Anchor.BottomLeft => new DefaultStageBorderLeft(SkinJson)
        {
            RelativeSizeAxes = Axes.None,
            Colour = ColourInfo.GradientVertical(Colour4.White, Colour4.White.Opacity(0)),
            Height = extended
        },
        Anchor.BottomCentre => new DefaultStageBackgroundBottom { Height = extended },
        Anchor.BottomRight => new DefaultStageBorderRight(SkinJson)
        {
            RelativeSizeAxes = Axes.None,
            Colour = ColourInfo.GradientVertical(Colour4.White, Colour4.White.Opacity(0)),
            Height = extended
        },
        _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
    };

    public Drawable GetLaneCover(bool bottom) => bottom ? new DefaultBottomLaneCover() : new DefaultTopLaneCover();

    public Drawable GetHealthBarBackground() => new DefaultHealthBackground();
    public Drawable GetHealthBar(HealthProcessor processor) => new DefaultHealthBar(SkinJson, processor);

    public virtual Drawable GetHitObject(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var piece = new DefaultHitObjectPiece(SkinJson, (MapColor)index);
        piece.UpdateColor(lane, keyCount);
        return piece;
    }

    public virtual Drawable GetTickNote(int lane, int keyCount, bool small) => new DefaultTickNote(small);

    public virtual Drawable GetLandmine(int lane, int keyCount, bool small) => new DefaultLandmine(small);

    public virtual Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var body = new DefaultHitObjectBody(SkinJson, (MapColor)index);
        body.UpdateColor(lane, keyCount);
        return body;
    }

    public virtual Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var end = new DefaultHitObjectEnd(SkinJson, (MapColor)index);
        end.UpdateColor(lane, keyCount);
        return end;
    }

    public virtual VisibilityContainer GetColumnLighting(int lane, int keyCount)
    {
        var lighting = new DefaultColumnLighting(SkinJson);
        lighting.UpdateColor(lane, keyCount);
        return lighting;
    }

    public virtual Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var receptor = down ? new DefaultReceptorDown(SkinJson, (MapColor)index) : new DefaultReceptorUp(SkinJson, (MapColor)index);
        receptor.UpdateColor(lane, keyCount);
        receptor.Height = SkinJson.GetKeymode(keyCount).HitPosition;
        return receptor;
    }

    public virtual Drawable GetHitLine() => new DefaultHitLine(SkinJson);
    public AbstractJudgementText GetJudgement(Judgement judgement, bool isLate) => new DefaultJudgementText(judgement, isLate);

    public Drawable GetFailFlash() => new Box();

    public Drawable GetResultsScoreRank(ScoreRank rank) => new DefaultResultsRank(rank);

    public Sample GetHitSample() => samples.Get("Gameplay/hitsound");
    public Sample[] GetMissSamples() => new[] { samples.Get("Gameplay/combobreak") };
    public Sample GetFailSample() => samples.Get("Gameplay/fail");
    public Sample GetRestartSample() => samples.Get("Gameplay/restart");
    public Sample GetFullComboSample() => samples.Get("Gameplay/full-combo");
    public Sample GetAllFlawlessSample() => samples.Get("Gameplay/all-flawless");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Textures?.Dispose();
        samples?.Dispose();
    }
}
