using System;
using System.Linq;
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

    public bool InBreak => Playfields.All(p => p.HitManager.Break);
    public bool AnyFailed => Players.Any(p => p.HealthProcessor.Failed);

    public event Action OnFinish;
    public bool Finished { get; private set; }

    public int Count { get; }

    public Playfield[] Playfields { get; private set; }
    public PlayfieldPlayer[] Players { get; private set; }

    public PlayfieldPlayer FirstPlayer => Players[0];

    public PlayfieldManager(DualMode mode)
    {
        Count = mode > DualMode.Disabled ? 2 : 1;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        screen.Hitsounding.PlayfieldCount = Count;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            Content = new[]
            {
                Players = Enumerable.Range(0, Count)
                                    .Select(i => new PlayfieldPlayer(i, screen.Map.ExtraPlayfields))
                                    .ToArray()
            }
        };

        Playfields = Players.Select(x => x.MainPlayfield).ToArray();
    }

    public bool OnComplete() => Players.All(p => p.HealthProcessor.OnComplete());

    protected override void Update()
    {
        base.Update();

        Players.ForEach(p => p.HealthProcessor.Update());

        if (!Finished && Playfields.All(p => p.HitManager.Finished))
        {
            OnFinish?.Invoke();
            Finished = true;
        }
    }
}
