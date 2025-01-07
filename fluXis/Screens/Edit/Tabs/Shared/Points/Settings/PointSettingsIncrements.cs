using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsIncrements : GridContainer
{
    public PointSettingsIncrements(EditorMap map, TimingPoint point)
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

        ColumnDimensions = new Dimension[]
        {
            new(),
            new(GridSizeMode.Absolute, 16),
            new()
        };

        Content = new[]
        {
            new[]
            {
                new IncrementSection("Offset", v =>
                {
                    point.Time += v;
                    map.Update(point);
                }),
                Empty(),
                new IncrementSection("BPM", v =>
                {
                    point.BPM = Math.Clamp(point.BPM + v, 1, 10000);
                    map.Update(point);
                }),
            }
        };
    }

    private partial class IncrementSection : CompositeDrawable
    {
        private string text { get; }
        private Action<int> change { get; }

        public IncrementSection(string text, Action<int> change)
        {
            this.text = text;
            this.change = change;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new IncrementButton(FontAwesome6.Solid.Minus, v => change(-v)),
                new FluXisSpriteText
                {
                    Text = text,
                    WebFontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new IncrementButton(FontAwesome6.Solid.Plus, v => change(v))
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                },
            };
        }
    }

    private partial class IncrementButton : CompositeDrawable
    {
        private Action<int> action { get; }

        private HoverLayer hover { get; }
        private FlashLayer flash { get; }

        private InputManager input;

        public IncrementButton(IconUsage icon, Action<int> action)
        {
            this.action = action;

            Width = 28;
            RelativeSizeAxes = Axes.Y;
            CornerRadius = 6;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(14),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override void LoadComplete()
        {
            input = GetContainingInputManager();
            base.LoadComplete();
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();

            var change = 1;
            change *= input.CurrentState.Keyboard.ControlPressed ? 10 : 1;
            change *= input.CurrentState.Keyboard.ShiftPressed ? 5 : 1;
            action?.Invoke(change);

            return true;
        }
    }
}
