using System;
using System.Linq;
using fluXis.Map;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class PlayfieldManager : CompositeDrawable
{
    [Resolved]
    private Hitsounding hitsounding { get; set; }

    public Bindable<bool> InBreak { get; } = new();
    public bool AnyFailed => Players.Any(p => p.HealthProcessor.Failed);

    public MapInfo MapInfo { get; }

    public event Action OnFinish;
    public bool Finished { get; private set; }

    public int Count { get; }

    public Playfield[] Playfields { get; private set; }
    public PlayfieldPlayer[] Players { get; private set; }

    public PlayfieldPlayer FirstPlayer => Players[0];

    public PlayfieldManager(MapInfo map)
    {
        MapInfo = map;
        Count = map.IsDual ? 2 : 1;

        Players = Enumerable.Range(0, Count)
                            .Select(i => new PlayfieldPlayer(i, MapInfo.ExtraPlayfields))
                            .ToArray();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        hitsounding.PlayfieldCount = Count;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            Content = new[] { Players }
        };

        Playfields = Players.Select(x => x.MainPlayfield).ToArray();
    }

    public bool OnComplete() => Players.All(p => p.HealthProcessor.OnComplete());

    protected override void Update()
    {
        base.Update();

        InBreak.Value = Playfields.All(p => p.HitManager.Break);

        Players.ForEach(p => p.HealthProcessor.Update(Time.Elapsed));

        if (!Finished && Playfields.All(p => p.HitManager.Finished))
        {
            OnFinish?.Invoke();
            Finished = true;
        }
    }
}
