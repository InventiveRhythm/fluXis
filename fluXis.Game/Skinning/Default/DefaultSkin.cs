using System;
using fluXis.Game.Audio;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Course;
using fluXis.Game.Skinning.Bases.Judgements;
using fluXis.Game.Skinning.Default.Health;
using fluXis.Game.Skinning.Default.HitObject;
using fluXis.Game.Skinning.Default.Judgements;
using fluXis.Game.Skinning.Default.Lighting;
using fluXis.Game.Skinning.Default.Receptor;
using fluXis.Game.Skinning.Default.Stage;
using fluXis.Game.Skinning.Json;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning.Default;

public class DefaultSkin : ISkin
{
    public SkinJson SkinJson { get; }
    private TextureStore textures { get; }
    private ISampleStore samples { get; }

    public const float BORDER_BASE = 8;
    public const float BORDER_COLOR = 4;

    public DefaultSkin(TextureStore textures, ISampleStore samples)
    {
        SkinJson = CreateJson();
        this.textures = textures;
        this.samples = samples;
    }

    protected virtual SkinJson CreateJson() => new DefaultSkinJson();

    public Texture GetDefaultBackground() => textures.Get("Backgrounds/default.png");

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

    public Drawable GetStageBackground() => new DefaultStageBackground();
    public Drawable GetStageBorder(bool right) => right ? new DefaultStageBorderRight(SkinJson) : new DefaultStageBorderLeft(SkinJson);
    public Drawable GetLaneCover(bool bottom) => bottom ? new DefaultBottomLaneCover() : new DefaultTopLaneCover();

    public Drawable GetHealthBarBackground() => new DefaultHealthBackground();
    public Drawable GetHealthBar(HealthProcessor processor) => new DefaultHealthBar(SkinJson, processor);

    public Drawable GetHitObject(int lane, int keyCount)
    {
        var piece = new DefaultHitObjectPiece(SkinJson);
        piece.UpdateColor(lane, keyCount);
        return piece;
    }

    public Drawable GetTickNote(int lane, int keyCount) => new DefaultTickNote();

    public Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var body = new DefaultHitObjectBody(SkinJson);
        body.UpdateColor(lane, keyCount);
        return body;
    }

    public Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var end = new DefaultHitObjectEnd(SkinJson);
        end.UpdateColor(lane, keyCount);
        return end;
    }

    public VisibilityContainer GetColumnLighting(int lane, int keyCount)
    {
        var lighting = new DefaultColumnLighting(SkinJson);
        lighting.UpdateColor(lane, keyCount);
        return lighting;
    }

    public Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var receptor = down ? new DefaultReceptorDown(SkinJson) : new DefaultReceptorUp(SkinJson);
        receptor.UpdateColor(lane, keyCount);
        receptor.Height = SkinJson.GetKeymode(keyCount).HitPosition;
        return receptor;
    }

    public Drawable GetHitLine() => new DefaultHitLine(SkinJson);
    public AbstractJudgementText GetJudgement(Judgement judgement, bool isLate) => new DefaultJudgementText(judgement, isLate);

    public Sample GetHitSample() => samples.Get("Gameplay/hitsound");
    public Sample[] GetMissSamples() => new[] { samples.Get("Gameplay/combobreak") };
    public Sample GetFailSample() => samples.Get("Gameplay/fail");
    public Sample GetRestartSample() => samples.Get("Gameplay/restart");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        textures?.Dispose();
        samples?.Dispose();
    }
}
