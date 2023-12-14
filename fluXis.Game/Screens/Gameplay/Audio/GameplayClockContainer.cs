using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Audio;

public partial class GameplayClockContainer : Container
{
    public GameplayClock GameplayClock { get; }

    public GameplayClockContainer(RealmMap realmMap, MapInfo info, Drawable[] drawables)
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            GameplayClock = new GameplayClock(info, realmMap.GetTrack()),
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Clock = GameplayClock,
                Children = drawables
            }
        };
    }
}
