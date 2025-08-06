using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Screens.Edit.Tabs.Charting;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Toolbox;

public partial class ToolboxButton : Container, IHasTooltip
{
    [Resolved]
    protected ChartingContainer ChartingContainer { get; private set; }

    public ChartingBlueprintContainer BlueprintContainer => ChartingContainer.BlueprintContainer;

    public ChartingTool Tool { get; init; }
    public virtual LocalisableString TooltipText => Tool.Description;

    protected virtual string Text => Tool.Name;
    protected virtual bool IsSelected => BlueprintContainer.CurrentTool == Tool;
    protected virtual bool PlayClickSound => true;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private HoverLayer hover;
    protected FlashLayer Flash { get; private set; }
    private Drawable icon;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

        Children = new Drawable[]
        {
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 5,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background4
                    },
                    hover = new HoverLayer(),
                    Flash = new FlashLayer(),
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(12, 0),
                        Padding = new MarginPadding(12),
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(24),
                                Child = icon = CreateIcon() ?? new FluXisSpriteIcon
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = FontAwesome6.Solid.Question
                                }
                            },
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = Text,
                                WebFontSize = 14
                            }
                        }
                    }
                }
            }
        };
    }

    [CanBeNull]
    protected virtual Drawable CreateIcon() => Tool.CreateIcon()?.With(i =>
    {
        i.RelativeSizeAxes = Axes.Both;
        i.Anchor = Anchor.Centre;
        i.Origin = Anchor.Centre;
    });

    public virtual void Select() => BlueprintContainer.CurrentTool = Tool;

    protected override void LoadComplete()
    {
        BlueprintContainer.CurrentToolChanged += UpdateSelectionState;
        UpdateSelectionState();
    }

    protected void UpdateSelectionState()
    {
        // bypass because it works different here
        TransformableExtensions.FadeTo(hover, IsSelected ? .1f : 0, 200);

        if (IsSelected)
            icon.ScaleTo(1, 800, Easing.OutElastic);
        else
            icon.ScaleTo(.8f, 200, Easing.OutQuint);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 100, Easing.OutElastic);
        base.OnMouseUp(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        TransformableExtensions.FadeTo(hover, IsSelected ? .1f : 0, 200);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Flash.Show();

        if (PlayClickSound)
            samples.Click();

        Select();
        return true;
    }
}
