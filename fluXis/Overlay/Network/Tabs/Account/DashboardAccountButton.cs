using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Account;

public partial class DashboardAccountButton : Container
{
    public string LabelText { get; set; }
    public string ButtonText { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(500, 50);
        Padding = new MarginPadding { Horizontal = 8 };

        Children = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = LabelText,
                WebFontSize = 16,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new FluXisButton
            {
                Text = ButtonText,
                Size = new Vector2(130, 32),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                FontSize = FluXisSpriteText.GetWebFontSize(14)
            }
        };
    }
}
