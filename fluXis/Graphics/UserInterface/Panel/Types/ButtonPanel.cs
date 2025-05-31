using System;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel.Types;

public partial class ButtonPanel : Panel, IKeyBindingHandler<FluXisGlobalKeybind>, ICloseable
{
    public IconUsage Icon { get; init; } = FontAwesome.Solid.QuestionCircle;
    public LocalisableString Text { get; init; }
    public LocalisableString SubText { get; init; }
    public ButtonData[] Buttons { get; init; } = Array.Empty<ButtonData>();
    public int AcceptIndex { get; init; }

    public Action<FluXisTextFlow> CreateSubText { get; set; }

    private FillFlowContainer<FluXisButton> buttons;
    private InputManager inputManager;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 490;
        AutoSizeAxes = Axes.Y;

        FluXisTextFlow subTextFlow;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;
        Content.Padding = new MarginPadding(0);
        Content.Children = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(20) { Top = 30 },
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Icon = Icon,
                        Size = new Vector2(64),
                        Margin = new MarginPadding { Bottom = 10 }
                    },
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        Text = Text,
                        FontSize = FluXisSpriteText.GetWebFontSize(20),
                        Shadow = false
                    },
                    subTextFlow = new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        FontSize = FluXisSpriteText.GetWebFontSize(14),
                        Alpha = string.IsNullOrEmpty(SubText.ToString()) && CreateSubText == null ? 0 : .8f,
                        Shadow = false
                    },
                    buttons = new FillFlowContainer<FluXisButton>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(10),
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding { Top = 20 },
                        ChildrenEnumerable = Buttons.Select(b => new FluXisButton
                        {
                            Width = 450,
                            Height = 50,
                            Data = b,
                            FontSize = FluXisSpriteText.GetWebFontSize(16),
                            Action = () =>
                            {
                                b.Action?.Invoke();
                                if (!Loading) Hide();
                            }
                        })
                    }
                }
            }
        };

        CreateSubText ??= f => f.Text = SubText;
        CreateSubText.Invoke(subTextFlow);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }

    protected override bool OnClick(ClickEvent e) => true;

    public void Close()
    {
        if (Loading)
            return;

        var last = Buttons.Last();
        last.Action?.Invoke();
        Hide();
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.Select:
                if (Buttons.Length < AcceptIndex + 1)
                    return false;

                var button = buttons[AcceptIndex];

                button.HoldToConfirm = false;
                button.TriggerClick();

                return true;

            default:
                return false;
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
