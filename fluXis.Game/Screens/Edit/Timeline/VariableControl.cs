using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class VariableControl : FillFlowContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    private BindableFloat rateBindable;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(5);
        Spacing = new Vector2(5);

        Children = new Drawable[]
        {
            new VariableControlContainer
            {
                Title = "Playback Speed",
                Bindable = rateBindable = new BindableFloat
                {
                    MinValue = .5f,
                    MaxValue = 3f,
                    Default = 1f,
                    Value = 1f,
                    Precision = .05f
                }
            },
            new VariableControlContainer
            {
                Title = "Zoom",
                Bindable = values.ZoomBindable
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        rateBindable.BindValueChanged(e => clock.Rate = e.NewValue, true);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;

        if (e.ControlPressed)
        {
            switch (e.Key)
            {
                case Key.Minus or Key.KeypadMinus:
                    rateBindable.Value -= rateBindable.Precision;
                    return true;

                case Key.Plus or Key.KeypadPlus:
                    rateBindable.Value += rateBindable.Precision;
                    return true;
            }
        }

        return false;
    }

    private partial class VariableControlContainer : Container
    {
        public string Title { get; set; }
        public BindableFloat Bindable { get; set; }

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
                                    Colour = FluXisColors.Background3
                                },
                                new ClickableSpriteIcon
                                {
                                    Icon = FontAwesome.Solid.Plus,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Size = new Vector2(10),
                                    Margin = new MarginPadding(5),
                                    Action = () => Bindable.Value += Bindable.Precision
                                },
                                new ClickableSpriteIcon
                                {
                                    Icon = FontAwesome.Solid.Minus,
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
