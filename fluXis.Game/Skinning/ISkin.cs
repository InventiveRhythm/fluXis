using System;
using fluXis.Game.Audio;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Skinning.Bases.Judgements;
using fluXis.Game.Skinning.Json;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning;

public interface ISkin : IDisposable
{
    public SkinJson SkinJson { get; }

    public Texture GetDefaultBackground();

    public Sample GetUISample(UISamples.SampleType type);

    public Drawable GetStageBackground();
    public Drawable GetStageBorder(bool right);
    public Drawable GetLaneCover(bool bottom);

    public Drawable GetHealthBarBackground();
    public Drawable GetHealthBar(HealthProcessor processor);

    public Drawable GetHitObject(int lane, int keyCount);
    public Drawable GetTickNote(int lane, int keyCount);
    public Drawable GetLongNoteBody(int lane, int keyCount);
    public Drawable GetLongNoteEnd(int lane, int keyCount);

    public VisibilityContainer GetColumnLighting(int lane, int keyCount);
    public Drawable GetReceptor(int lane, int keyCount, bool down);
    public Drawable GetHitLine();

    public AbstractJudgementText GetJudgement(Judgement judgement, bool isLate);

    public Sample GetHitSample();
    public Sample[] GetMissSamples();
    public Sample GetFailSample();
    public Sample GetRestartSample();
}
