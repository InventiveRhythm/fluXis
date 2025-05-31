using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.TabSwitcher;

public partial class EditorTabSwitcherButton : ClickableContainer
{
    private IconUsage icon { get; }
    private string text { get; }

    [Resolved]
    private UISamples samples { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;

    public EditorTabSwitcherButton(IconUsage icon, string text, Action action)
    {
        this.icon = icon;
        this.text = text;
        Action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
        Children = new Drawable[]
        {
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = icon,
                        Size = new Vector2(20),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding(12)
                    },
                    new FluXisSpriteText
                    {
                        Text = text,
                        WebFontSize = 16,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Right = 12 }
                    }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        samples.Click();
        Action?.Invoke();
        return true;
    }
}
