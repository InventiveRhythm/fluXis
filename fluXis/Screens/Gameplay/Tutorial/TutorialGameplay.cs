using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Tutorial;

public partial class TutorialGameplay : GameplayScreen
{
    public override bool PlayBackSound => false;

    protected override double GameplayStartTime => -4000;
    public override bool FadeBackToGlobalClock => false;
    protected override bool AllowRestart => false;
    protected override bool AllowPausing => false;
    protected override bool DisplayHUD => false;
    protected override bool SubmitScore => false;

    private Box blackOverlay;

    public TutorialGameplay(RealmMap realmMap)
        : base(realmMap, new List<IMod> { new NoFailMod() })
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(blackOverlay = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = 0
        });
    }

    protected override RulesetContainer CreateRuleset()
    {
        var ruleset = base.CreateRuleset();
        ruleset.AlwaysShowKeys = true;
        return ruleset;
    }

    protected override void End()
    {
        GameplayClock.Track.VolumeTo(0, 4000);
        blackOverlay.FadeInFromZero(4000).Then(1200).Schedule(this.Exit);
    }
}
