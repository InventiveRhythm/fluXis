using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Localization;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Account;

public partial class DashboardAccountLogoutButton : CompositeDrawable
{
    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private FluXisGame game { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(36);
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        Margin = new MarginPadding { Right = 20 };
        CornerRadius = 36 / 2f;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            new FluXisSpriteIcon
            {
                Size = new Vector2(16),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome6.Solid.PersonToDoor
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();

        panels.Content = new ButtonPanel
        {
            Text = "Are you sure you want to log out?",
            SubText = "This will exit the game.",
            Icon = FontAwesome6.Solid.PersonToDoor,
            Buttons = new ButtonData[]
            {
                new DangerButtonData(LocalizationStrings.General.PanelGenericConfirm, () =>
                {
                    api.Logout();
                    game.Exit();
                }, true),
                new CancelButtonData()
            }
        };

        return true;
    }
}
