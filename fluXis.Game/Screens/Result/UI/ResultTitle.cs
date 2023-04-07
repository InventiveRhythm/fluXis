using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultTitle : FillFlowContainer
{
    public ResultTitle(RealmMap map)
    {
        Direction = FillDirection.Vertical;
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;

        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = map.Metadata.Title,
                Font = FluXisFont.Default(40),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new SpriteText
            {
                Text = map.Metadata.Artist,
                Font = FluXisFont.Default(32),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new SpriteText
            {
                Text = $"[{map.Difficulty}] mapped by {map.Metadata.Mapper}",
                Font = FluXisFont.Default(24),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            }
        });
    }
}
