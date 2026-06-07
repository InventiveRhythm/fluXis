using System;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public class TutorialCapability : IEndingCapability, IRulesetCapability
{
    public GameplayScreen Screen { get; set; } = null!;

    private Box blackOverlay = null!;

    void IGameplayCapability.PreLoad()
    {
        Screen.AllowPausing = false;
        Screen.AllowRestarting = false;
        Screen.FadeBackToGlobalClock = false;
        Screen.GameplayStartTime = -4000;
    }

    void IGameplayCapability.PostLoad()
    {
        Screen.Add(blackOverlay = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = 0
        });
    }

    Screen? IEndingCapability.OnEnd(ScoreInfo score, Action complete)
    {
        Screen.GameplayClock.Track.VolumeTo(0, 4000);
        blackOverlay.FadeInFromZero(4000).Then(1200).Schedule(Screen.Exit);
        return null;
    }

    void IRulesetCapability.Modify(RulesetContainer ruleset) => ruleset.AlwaysShowKeys = true;
}
