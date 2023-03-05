using fluXis.Game.Database.Maps;
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
                Font = new FontUsage("Quicksand", 40, "Bold"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new SpriteText
            {
                Text = map.Metadata.Artist,
                Font = new FontUsage("Quicksand", 32, "Bold"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new SpriteText
            {
                Text = $"[{map.Difficulty}] mapped by {map.Metadata.Mapper}",
                Font = new FontUsage("Quicksand", 24, "SemiBold"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            }
        });
    }
}