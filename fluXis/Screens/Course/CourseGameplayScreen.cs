using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay;
using osu.Framework.Screens;

namespace fluXis.Screens.Course;

public partial class CourseGameplayScreen : GameplayScreen
{
    public event Action<ScoreInfo> OnResults;

    public CourseGameplayScreen(RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
    }

    protected override void End()
    {
        OnResults?.Invoke(PlayfieldManager.FirstPlayer.ScoreProcessor.ToScoreInfo());
        this.Exit();
    }
}
