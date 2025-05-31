using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Overlay.Network;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.User.Header;

public partial class HeaderEditButton : CircularContainer
{
    [Resolved]
    private UISamples samples { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 48;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Colour = FluXisColors.Text,
                Padding = new MarginPadding { Horizontal = 20 },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(20),
                        Child = new FluXisSpriteIcon
                        {
                            Icon = FontAwesome6.Solid.Pen,
                            Size = new Vector2(20)
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = "Edit",
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.Show();

        game?.OpenDashboard(DashboardTabType.Account);
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.Show();

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
