using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Panel.Parts;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class EasingSelector : Panel, ICloseable
{
    public EasingSelector(Action<Easing> pick)
    {
        Size = new Vector2(1600, 900);

        Content.Child = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding(16),
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions =
                [
                    new Dimension(GridSizeMode.Absolute, PanelHeader.HEIGHT),
                    new Dimension(GridSizeMode.Absolute, 16),
                    new Dimension()
                ],
                Content = new[]
                {
                    new Drawable[] { new PanelHeader(Phosphor.Bold.Speedometer, "Easings", Close) },
                    new[] { Empty() },
                    new Drawable[]
                    {
                        new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false,
                            ScrollbarOverlapsContent = true,
                            Child = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Full,
                                Spacing = new Vector2(16),
                                ChildrenEnumerable = Enum.GetValues<Easing>().Select(x => new Tile(x)
                                {
                                    Action = () =>
                                    {
                                        pick.Invoke(x);
                                        Close();
                                    }
                                })
                            }
                        }
                    },
                }
            }
        };
    }

    public void Close() => Hide();

    private partial class Tile : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private const float container_width = 280;
        private const float box_size = 44;

        private readonly Easing ease;

        private readonly Container content;
        private readonly HoverLayer hover;
        private readonly FlashLayer flash;
        private readonly Drawable box;

        public Action Action { get; init; }

        public Tile(Easing ease)
        {
            this.ease = ease;

            Height = 160;

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 12,
                Masking = true,
                Children =
                [
                    new Box { Colour = Theme.Background2 }.WithRelativeSize(Axes.Both),
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(16),
                        Children =
                        [
                            new FluXisSpriteText
                            {
                                WebFontSize = 20,
                                Text = ease.ToString(),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            new Container
                            {
                                Size = new Vector2(container_width, box_size),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Child = box = new Container
                                {
                                    Size = new Vector2(box_size),
                                    CornerRadius = 8,
                                    Masking = true,
                                    Colour = Theme.Highlight,
                                    Child = new Box().WithRelativeSize(Axes.Both)
                                }
                            }
                        ]
                    }
                ]
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            box.MoveToX(container_width - box_size, 1200, ease).Then(400)
               .MoveToX(0, 1200, ease).Then(400).Loop();
        }

        protected override void Update()
        {
            base.Update();
            Width = ((Parent?.DrawWidth ?? 100) - 16 * 3) / 4;
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
            => hover.Hide();

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.9f, 1000, Easing.OutQuint);
            return false;
        }

        protected override void OnMouseUp(MouseUpEvent e)
            => content.ScaleTo(1f, 1000, Easing.OutElasticHalf);

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            Action?.Invoke();
            return true;
        }
    }
}
