using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Settings.Tabs;

public partial class SettingsCategoryTab : Container
{
    private SettingsSection section { get; }
    private Bindable<SettingsSection> currentSection { get; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container scalingContainer;
    private FillFlowContainer content;
    private HoverLayer hover;
    private FlashLayer flash;

    public SettingsCategoryTab(SettingsSection section, Bindable<SettingsSection> bind)
    {
        this.section = section;
        currentSection = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(70);

        InternalChild = scalingContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(20),
                    Padding = new MarginPadding(20),
                    Children = new Drawable[]
                    {
                        new FluXisSpriteIcon
                        {
                            Size = new Vector2(30),
                            Icon = section.Icon,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        },
                        new FluXisSpriteText
                        {
                            Text = section.Title,
                            FontSize = 24,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        currentSection.BindValueChanged(e =>
        {
            ScheduleAfterChildren(() =>
            {
                if (e.NewValue == section)
                    select();
                else
                    deselect();
            });
        }, true);
    }

    private void select() => this.ResizeWidthTo(content.DrawWidth, 400, Easing.OutQuint);
    private void deselect() => this.ResizeWidthTo(70, 400, Easing.OutQuint);

    protected override bool OnClick(ClickEvent e)
    {
        currentSection.Value = section;
        samples.Click();
        flash.Show();
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
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
