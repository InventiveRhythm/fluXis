using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Audio;

public partial class GameplayClockContainer : Container
{
    public GameplayClock GameplayClock { get; }
    private Container container { get; }

    private DependencyContainer dependencies;

    public GameplayClockContainer(ITrackStore tracks, RealmMap realmMap, MapInfo info, IEnumerable<Drawable> drawables, bool useOffset = true)
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        GameplayClock = new GameplayClock(tracks, info, realmMap.GetTrack(), realmMap.Settings.Offset, useOffset);

        InternalChildren = new Drawable[]
        {
            GameplayClock,
            container = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Clock = GameplayClock,
                ChildrenEnumerable = drawables
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        dependencies.CacheAs<IBeatSyncProvider>(GameplayClock);
    }

    public override void Add(Drawable drawable)
    {
        container.Add(drawable);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
