using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsCategoryTab : Container
{
    public SettingsSection Section { get; init; }
    private readonly SettingsMenu menu;

    [Resolved]
    private UISamples samples { get; set; }

    private Container scalingContainer;
    private FillFlowContainer content;
    private Box hover;
    private Box flash;

    public SettingsCategoryTab(SettingsMenu menu)
    {
        this.menu = menu;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(50);

        InternalChild = scalingContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    Height = 50,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5, 0),
                    Padding = new MarginPadding(5) { Right = 10 },
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Size = new Vector2(30),
                            Icon = Section.Icon,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Margin = new MarginPadding { Horizontal = 5 }
                        },
                        new FluXisSpriteText
                        {
                            Text = Section.Title,
                            FontSize = 24,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        menu.Selector.SelectTab(this);
        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        scalingContainer.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        scalingContainer.ScaleTo(1f, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    public void Select() => this.ResizeWidthTo(content.DrawWidth, 400, Easing.OutQuint);
    public void Deselect() => this.ResizeWidthTo(50, 400, Easing.OutQuint);
}
