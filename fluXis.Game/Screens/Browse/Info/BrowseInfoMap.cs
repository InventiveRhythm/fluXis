using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Browse.Info;

public partial class BrowseInfoMap : Container, IHasTooltip
{
    public LocalisableString TooltipText => map.Difficulty;

    private bool mapperIsCreator => map.Mapper.ID == set.Creator.ID;

    private readonly APIMapSet set;
    private readonly APIMap map;

    public BrowseInfoMap(APIMapSet set, APIMap map)
    {
        this.set = set;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 760;
        Height = 50;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new KeyModeIcon
            {
                KeyMode = map.Mode,
                Size = new Vector2(50)
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Left = 60, Right = 110 },
                Children = new Drawable[]
                {
                    new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = map.Difficulty,
                        FontSize = mapperIsCreator ? 24 : 20,
                    },
                    new TruncatingText
                    {
                        RelativeSizeAxes = Axes.X,
                        Text = $"mapped by {map.Mapper.Username}",
                        FontSize = 14,
                        Alpha = mapperIsCreator ? 0 : 1,
                        Colour = FluXisColors.Text2
                    }
                }
            },
            new DifficultyChip
            {
                Rating = (float)map.Rating,
                Width = 70,
                Height = 20,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                X = -15
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }
}
