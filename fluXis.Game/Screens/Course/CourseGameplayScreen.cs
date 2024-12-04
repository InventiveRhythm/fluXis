using System;
using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Mods;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Course;

public partial class CourseGameplayScreen : GameplayScreen
{
    public event Action<ScoreInfo> OnResults;

    public CourseGameplayScreen(RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
    }

    protected override void End()
    {
        OnResults?.Invoke(PlayfieldManager.Playfields[0].ScoreProcessor.ToScoreInfo());
        this.Exit();
    }
}
