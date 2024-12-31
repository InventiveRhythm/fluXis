using System;
using System.Linq;
using fluXis.Configuration.Experiments;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class PlayfieldManager : CompositeDrawable
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    public bool InBreak => Playfields.All(p => p.Manager.Break);
    public bool AnyFailed => Playfields.Any(p => p.HealthProcessor.Failed);

    public event Action OnFinish;
    public bool Finished { get; private set; }

    public int Count { get; }
    public Playfield[] Playfields { get; private set; }

    public PlayfieldManager(DualMode mode)
    {
        Count = mode > DualMode.Disabled ? 2 : 1;
    }

    [BackgroundDependencyLoader]
    private void load(ExperimentConfigManager experiments)
    {
        RelativeSizeAxes = Axes.Both;
        screen.Hitsounding.PlayfieldCount = Count;

        var canSeek = experiments.Get<bool>(ExperimentConfig.Seeking);

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            Content = new[]
            {
                Playfields = Enumerable.Range(0, Count)
                                       .Select(i => new Playfield(i, canSeek))
                                       .ToArray()
            }
        };
    }

    public bool OnComplete() => Playfields.All(p => p.HealthProcessor.OnComplete());

    protected override void Update()
    {
        base.Update();

        Playfields.ForEach(p => p.HealthProcessor.Update());

        if (!Finished && Playfields.All(p => p.Manager.Finished))
        {
            OnFinish?.Invoke();
            Finished = true;
        }
    }
}
