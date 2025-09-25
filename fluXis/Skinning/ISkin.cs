using System;
using fluXis.Audio;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Course;
using fluXis.Skinning.Bases.Judgements;
using fluXis.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning;

public interface ISkin : IDisposable
{
    SkinJson SkinJson { get; }

    Texture GetIcon();
    Texture GetDefaultBackground();

    Sample GetUISample(UISamples.SampleType type);
    Sample GetCourseSample(CourseScreen.SampleType type);

    Drawable GetStageBackgroundPart(Anchor part);
    Drawable GetLaneCover(bool bottom);

    Drawable GetHealthBarBackground();
    Drawable GetHealthBar(HealthProcessor processor);

    Drawable GetHitObject(int lane, int keyCount);
    Drawable GetTickNote(int lane, int keyCount, bool small);
    Drawable GetLandmine(int lane, int keyCount, bool small);
    Drawable GetLongNoteBody(int lane, int keyCount);
    Drawable GetLongNoteEnd(int lane, int keyCount);

    VisibilityContainer GetColumnLighting(int lane, int keyCount);
    Drawable GetReceptor(int lane, int keyCount, bool down);
    Drawable GetHitLine();

    AbstractJudgementText GetJudgement(Judgement judgement, bool isLate);

    Drawable GetFailFlash();

    Drawable GetResultsScoreRank(ScoreRank rank);

    Sample GetHitSample();
    Sample[] GetMissSamples();
    Sample GetFailSample();
    Sample GetRestartSample();
    Sample GetFullComboSample();
    Sample GetAllFlawlessSample();
}
