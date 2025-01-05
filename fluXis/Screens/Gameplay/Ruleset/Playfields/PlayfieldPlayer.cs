using System.Linq;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class PlayfieldPlayer : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GameplaySamples samples { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    public Playfield MainPlayfield { get; }
    public Playfield[] SubPlayfields { get; }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    private DependencyContainer dependencies;

    public PlayfieldPlayer(int index, int subCount)
    {
        MainPlayfield = new Playfield(index, 0);
        SubPlayfields = Enumerable.Range(1, subCount).Select(x => new Playfield(index, x)).ToArray();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        dependencies.CacheAs(this);

        JudgementProcessor.AddDependants(new JudgementDependant[]
        {
            HealthProcessor = ruleset.CreateHealthProcessor(),
            ScoreProcessor = new ScoreProcessor
            {
                Player = /*screen.CurrentPlayer ?? */APIUser.Default,
                HitWindows = ruleset.HitWindows,
                MapInfo = ruleset.MapInfo,
                Mods = ruleset.Mods
            }
        });

        InternalChild = MainPlayfield;
        AddRangeInternal(SubPlayfields);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        JudgementProcessor.ApplyMap(ruleset.MapInfo);
        ScoreProcessor.OnComboBreak += () =>
        {
            if (ruleset.CatchingUp)
                return;

            samples?.Miss();
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
