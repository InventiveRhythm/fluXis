using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.UI.BottomBar;

public partial class VariableControl : FillFlowContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(10);
        Spacing = new Vector2(5);

        Children = new Drawable[]
        {
            new VariableControlContainer
            {
                Title = "Playback Speed",
                Bindable = clock.RateBindable
            },
            new VariableControlContainer
            {
                Title = "Zoom",
                Bindable = settings.ZoomBindable
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        clock.RateBindable.BindValueChanged(e => clock.Rate = e.NewValue, true);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;
        if (!e.ControlPressed) return false;

        switch (e.Key)
        {
            case Key.Minus or Key.KeypadMinus:
                clock.Rate -= .05f;
                return true;

            case Key.Plus or Key.KeypadPlus:
                clock.Rate += .05f;
                return true;
        }

        return false;
    }

    private partial class VariableControlContainer : Container
    {
        public string Title { get; init; }
        public BindableNumber<double> Bindable { get; init; }

        private FluXisSpriteText valueText;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 20;
            Anchor = Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = Title
                },
                new FillFlowContainer
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Size = new Vector2(40, 20),
                            CornerRadius = 5,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background3
                                },
                                new ClickableSpriteIcon
                                {
                                    Icon = FontAwesome6.Solid.Plus,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Size = new Vector2(10),
                                    Margin = new MarginPadding(5),
                                    Action = () => Bindable.Value += Bindable.Precision
                                },
                                new ClickableSpriteIcon
                                {
                                    Icon = FontAwesome6.Solid.Minus,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Size = new Vector2(10),
                                    Margin = new MarginPadding(5),
                                    Action = () => Bindable.Value -= Bindable.Precision
                                }
                            }
                        },
                        valueText = new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            Bindable.BindValueChanged(e =>
            {
                var percent = e.NewValue / Bindable.Default;
                valueText.Text = $"{(int)Math.Round(percent * 100)}%";
            }, true);
        }
    }
}
