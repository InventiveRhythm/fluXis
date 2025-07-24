using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownList : MarkdownList
{
    public FluXisMarkdownList()
    {
        Spacing = new Vector2();
        Margin = new MarginPadding { Bottom = 16 };
    }

    public override void Add(Drawable drawable)
    {
        if (drawable is null)
            return;

        drawable.Margin = new MarginPadding();
        drawable.X = 16;
        base.Add(new Container
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Children = new[]
            {
                new Circle
                {
                    Size = new Vector2(6),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Colour = Theme.Text,
                    X = 4,
                },
                drawable
            }
        });
    }
}
