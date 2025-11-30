using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.User.Sidebar;

public partial class ProfileAboutMe : FillFlowContainer
{
    public ProfileAboutMe(string content)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Alpha = string.IsNullOrWhiteSpace(content) ? 0f : 1f;
        Spacing = new Vector2(2);

        InternalChildren = new Drawable[]
        {
            new ForcedHeightText
            {
                Text = "About Me",
                Height = 28,
                WebFontSize = 20
            },
            new FluXisTextFlow
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                WebFontSize = 16,
                Text = content,
                Alpha = .8f
            }
        };
    }
}
