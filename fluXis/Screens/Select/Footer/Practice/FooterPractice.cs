using System;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Footer.Practice;

public partial class FooterPractice : FocusedOverlayContainer
{
    protected override bool StartHidden => true;
    public FooterButton Button { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    public Action<int, int> PracticeAction { get; init; }

    private ForcedHeightText text;

    private FooterPracticeGraph practiceGraph;

    private readonly BindableNumber<int> start = new();
    private readonly BindableNumber<int> end = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 600;
        AutoSizeAxes = Axes.Y;
        Margin = new MarginPadding { Bottom = 100 };
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomCentre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(40),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomRight,
                Rotation = 45,
                Y = 20,
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Masking = true,
                CornerRadius = 12,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(16),
                        Spacing = new Vector2(16),
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Spacing = new Vector2(8),
                                Direction = FillDirection.Vertical,
                                Children = new Drawable[]
                                {
                                    new ForcedHeightText(true)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Text = "Practice Mode",
                                        WebFontSize = 20,
                                        Height = 15
                                    },
                                    text = new ForcedHeightText(true)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        WebFontSize = 14,
                                        Height = 10,
                                        Alpha = .8f
                                    }
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 80,
                                Children = new Drawable[]
                                {
                                    practiceGraph = new FooterPracticeGraph(start, end)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 80,
                                    },
                                    new FooterPracticePlayhead(start, end)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 85,
                                    },
                                    new FooterPracticeRangeController(start, end, practiceGraph.Bars[^1].Parent.X)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 60,
                                        Margin = new MarginPadding { Vertical = 10 }
                                    }
                                }
                            },
                            new FillFlowContainer()
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 75,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(12),
                                Children = new Drawable[]
                                {
                                    new FooterPracticeControl("Start", start, end) { Width = 278 },
                                    new FooterPracticeControl("End", end, start) { Width = 278 }
                                }
                            },
                            new FillFlowContainer()
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(12),
                                Children = new Drawable[]
                                {
                                    new FluXisButton
                                    {
                                        Text = "Reset",
                                        FontSize = FluXisSpriteText.GetWebFontSize(16),
                                        Color = Theme.Background3,
                                        Size = new Vector2(278, 50),
                                        Action = () =>
                                        {
                                            this.TransformBindableTo(start, start.MinValue, 200, Easing.OutQuint);
                                            this.TransformBindableTo(end, end.MaxValue, 200, Easing.OutQuint);
                                        }
                                    },
                                    new FluXisButton
                                    {
                                        Text = "Start!",
                                        FontSize = FluXisSpriteText.GetWebFontSize(16),
                                        Color = Theme.Highlight,
                                        TextColor = Theme.Background2,
                                        Size = new Vector2(278, 50),
                                        Action = () => PracticeAction?.Invoke(start.Value * 1000, end.Value * 1000)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);

        start.ValueChanged += _ => end.MinValue = start.Value + 1;
        end.ValueChanged += _ => start.MaxValue = end.Value - 1;
    }

    private void mapChanged(ValueChangedEvent<RealmMap> e)
    {
        var map = e.NewValue;

        text.Text = $"{map.Metadata.LocalizedArtist} - {map.Metadata.LocalizedTitle} [{map.Difficulty}]";

        if (e.OldValue.FullAudioPath == map.FullAudioPath && end.MaxValue != int.MaxValue)
            return;

        var max = (int)Math.Max(Math.Ceiling(map.Filters.Length / 1000), 1);

        start.Value = 0;
        end.Value = 1;

        start.MinValue = 0;
        end.Value = end.MaxValue = max;

        start.Value = start.MinValue;

        end.MinValue = start.Value + 1;
        start.MaxValue = end.Value - 1;
    }

    protected override void Update()
    {
        base.Update();

        var delta = Button.ScreenSpaceDrawQuad.Centre.X - ScreenSpaceDrawQuad.Centre.X;
        X += delta;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnClick(ClickEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    protected override void OnFocusLost(FocusLostEvent e) => Hide();

    protected override void PopIn() => this.FadeIn(200).MoveToY(0, 400, Easing.OutQuint);
    protected override void PopOut() => this.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
}
