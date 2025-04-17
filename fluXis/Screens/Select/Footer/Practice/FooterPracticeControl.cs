﻿using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPracticeControl : GridContainer
{
    private BindableNumber<int> bind { get; }
    private BindableNumber<int> other { get; }

    private FluXisSpriteText time { get; }
    private ForcedHeightText percentage { get; }

    private InputManager input;

    public FooterPracticeControl(string title, BindableNumber<int> bind, BindableNumber<int> other)
    {
        this.bind = bind;
        this.other = other;

        Width = 250;
        RelativeSizeAxes = Axes.Y;
        ColumnDimensions = new Dimension[]
        {
            new(GridSizeMode.Absolute, 32),
            new(),
            new(GridSizeMode.Absolute, 32)
        };

        Content = new[]
        {
            new Drawable[]
            {
                new Button(FontAwesome6.Solid.Minus, () => changeValue(-1)),
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        new ForcedHeightText
                        {
                            Text = title,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 12,
                            Height = 9,
                            Alpha = .6f
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.X,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = 18,
                            Child = time = new FluXisSpriteText
                            {
                                Text = "1:24",
                                WebFontSize = 24,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }
                        },
                        percentage = new ForcedHeightText
                        {
                            Text = "50%",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 16,
                            Height = 12,
                            Alpha = .8f
                        }
                    }
                },
                new Button(FontAwesome6.Solid.Plus, () => changeValue(1))
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        input = GetContainingInputManager();
    }

    private bool changeValue(int change)
    {
        var shift = input.CurrentState.Keyboard.ShiftPressed ? 10 : 1;
        var ctrl = input.CurrentState.Keyboard.ControlPressed ? 6 : 1;

        var val = bind.Value;
        bind.Value += change * shift * ctrl;

        var changed = val != bind.Value;
        time.MoveToY(-change * (changed ? 6 : 2)).MoveToY(0, 600, Easing.OutElasticQuarter);
        return changed;
    }

    protected override void Update()
    {
        base.Update();

        var max = Math.Max(bind.MaxValue, other.MaxValue);

        time.Text = TimeUtils.Format(bind.Value * 1000, false);
        percentage.Text = $"{bind.Value / (float)max * 100:0}%";
    }

    private partial class Button : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private Func<bool> action { get; }
        private HoverLayer hover { get; }
        private FlashLayer flash { get; }

        public Button(IconUsage icon, Func<bool> action)
        {
            this.action = action;

            RelativeSizeAxes = Axes.Both;
            CornerRadius = 8;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(16)
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();
            samples.Click(!(action?.Invoke() ?? true));
            return false;
        }
    }
}
