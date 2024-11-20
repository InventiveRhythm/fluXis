using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Network.Sidebar;

public partial class DashboardSidebarButton : Container
{
    public DashboardTab Tab { get; init; }
    public Action<DashboardTab> SelectAction { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;
    private SpriteIcon icon;

    public bool Selected
    {
        set => icon.FadeTo(value ? 1 : .5f, 200);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 64;

        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 15,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        icon = new FluXisSpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(28),
                            Margin = new MarginPadding(18),
                            Icon = Tab.Icon,
                            Alpha = .5f
                        },
                        new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = Tab.Title
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        SelectAction?.Invoke(Tab);
        samples.Click();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1f, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
