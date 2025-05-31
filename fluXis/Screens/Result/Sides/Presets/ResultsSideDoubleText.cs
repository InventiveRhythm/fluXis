using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Result.Sides.Presets;

public partial class ResultsSideDoubleText : CompositeDrawable
{
    private string title { get; }
    private string value { get; }

    public ResultsSideDoubleText(string title, string value)
    {
        this.title = title;
        this.value = value;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 12;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = title,
                WebFontSize = 16,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new FluXisSpriteText
            {
                Text = value,
                WebFontSize = 16,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            }
        };
    }
}
