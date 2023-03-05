using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Online.Overlay;

public partial class OnlineOverlay : CompositeDrawable
{
    public bool Visible;

    public LoginOverlay LoginOverlay;
    public OnlineSidebar Sidebar;
    public OverlayChat Chat;

    private Box background;
    private FillFlowContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0
            },
            content = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Alpha = 0,
                Scale = new Vector2(.96f),
                Padding = new MarginPadding(20),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    Sidebar = new OnlineSidebar(),
                    Chat = new OverlayChat()
                }
            },
            LoginOverlay = new LoginOverlay(this)
        };
    }

    public void ToggleVisibility()
    {
        if (Visible)
        {
            background.FadeOut(200);
            content.ScaleTo(.96f, 200, Easing.InQuint).FadeOut(200);
            LoginOverlay.Hide();
        }
        else
        {
            background.FadeTo(.4f, 200);
            content.ScaleTo(1f, 800, Easing.OutElastic).FadeIn(400);
            LoginOverlay.Show();
        }

        Visible = !Visible;
    }

    protected override bool OnClick(ClickEvent e)
    {
        // only handle if we're visible
        return Visible;
    }

    protected override bool OnHover(HoverEvent e)
    {
        // only handle if we're visible
        return Visible;
    }

    public void OnUserLogin()
    {
        Sidebar.OnUserLogin();
    }
}