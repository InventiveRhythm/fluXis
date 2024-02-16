using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxButton : Container, IHasTextTooltip
{
    [Resolved]
    protected ChartingContainer ChartingContainer { get; private set; }

    public BlueprintContainer BlueprintContainer => ChartingContainer.BlueprintContainer;

    public ChartingTool Tool { get; init; }
    public virtual LocalisableString Tooltip => Tool.Description;

    public virtual string Text => Tool.Name;
    public virtual bool IsSelected => BlueprintContainer.CurrentTool == Tool;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private Box hover;
    private Box flash;
    private Drawable icon;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 64;

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
                        Colour = FluXisColors.Background4
                    },
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
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(16, 0),
                        Padding = new MarginPadding(16),
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(32),
                                Child = icon = CreateIcon() ?? new SpriteIcon
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
                                FontSize = 28
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
        hover.FadeTo(IsSelected ? .1f : 0, 200);

        if (IsSelected)
            icon.ScaleTo(1, 800, Easing.OutElastic);
        else
            icon.ScaleTo(.8f, 200, Easing.OutQuint);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 4000, Easing.OutQuint);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
        base.OnMouseUp(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeTo(IsSelected ? .1f : 0, 200);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(400);
        samples.Click();
        Select();
        return true;
    }
}
