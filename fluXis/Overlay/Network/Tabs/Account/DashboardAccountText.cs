using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Network.Tabs.Account;

public partial class DashboardAccountText : Container
{
    public string Title { get; set; }
    public string Value { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                X = 140,
                FontSize = 16,
                Text = Title
            },
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                X = 150,
                Text = Value
            }
        };
    }
}
