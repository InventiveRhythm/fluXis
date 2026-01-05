using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationRow : GridContainer
{
    private readonly StoryboardElement item;
    private readonly StoryboardAnimationType type;

    public StoryboardAnimationRow(StoryboardElement item, StoryboardAnimationType type)
    {
        this.item = item;
        this.type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = StoryboardAnimationsList.ROW_HEIGHT;
        ColumnDimensions = new Dimension[]
        {
            new(GridSizeMode.Absolute, StoryboardAnimationsList.SIDE_WIDTH),
            new()
        };

        Content = new[]
        {
            new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"{type}",
                    WebFontSize = 12,
                    Margin = new MarginPadding { Horizontal = 8 },
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    ChildrenEnumerable = item.Animations.Where(x => x.Type == type).Select(x => new StoryboardAnimationEntry(x, this))
                }
            }
        };
    }
}
