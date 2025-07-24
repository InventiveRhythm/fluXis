using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.UI;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Gameplay.UI.Menus;

public partial class GameplayMenuButton : Container, IStateful<SelectedState>
{
    public string Text { get; init; }
    public string SubText { get; init; }
    public IconUsage Icon { get; init; }
    public Action Action { get; init; }
    public Colour4 Color { get; init; } = Theme.Text;

    private SelectedState state;

    public SelectedState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            StateChanged?.Invoke(state);
            updateState();
        }
    }

    [CanBeNull]
    public event Action<SelectedState> StateChanged;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 80;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background3
                },
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(30),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 20 },
                    Colour = Color
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Direction = FillDirection.Vertical,
                    Margin = new MarginPadding { Left = 70 },
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = Text,
                            Margin = new MarginPadding { Bottom = -5 },
                            FontSize = 26,
                            Colour = Color
                        },
                        new FluXisSpriteText
                        {
                            Text = SubText,
                            Colour = Color,
                            Alpha = .6f
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        Action?.Invoke();
        samples.Click();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        State = SelectedState.Selected;
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        State = SelectedState.Deselected;
    }

    private void updateState()
    {
        switch (state)
        {
            case SelectedState.Selected:
                hover.Show();
                samples.Hover();
                break;

            case SelectedState.Deselected:
                hover.Hide();
                break;
        }
    }
}
