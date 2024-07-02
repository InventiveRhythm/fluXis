using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.MapSet.Buttons;
using fluXis.Game.Overlay.MapSet.UI.Difficulties;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.MapSet;

public partial class MapSetOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; }

    private APIMapSet set;
    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                Width = 1320,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background1
                        },
                        scroll = new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false,
                            Child = flow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Top = 70, Bottom = 20, Horizontal = 20 },
                                Spacing = new Vector2(20)
                            }
                        }
                    }
                }
            }
        };
    }

    public void ShowSet(long id)
    {
        Show();

        if (set?.ID != id)
            fetch(id);
    }

    private async void fetch(long id)
    {
        var request = new MapSetRequest(id);
        await api.PerformRequestAsync(request);

        // we might want to show a message if the request fails
        if (!request.IsSuccessful)
            return;

        set = request.Response!.Data;
        Schedule(() => displayData(set));
    }

    private void displayData(APIMapSet set)
    {
        set.Maps.Sort((a, b) => a.NotesPerSecond.CompareTo(b.NotesPerSecond));
        var bindableMap = new Bindable<APIMap>(set.Maps.First());

        flow.Clear();
        flow.Children = new Drawable[]
        {
            new MapSetHeader(set),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 10, Bottom = 20 },
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Spacing = new Vector2(12),
                                ChildrenEnumerable = set.Maps.Select(m => new DifficultyChip(set, m, bindableMap))
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Spacing = new Vector2(12),
                                Children = new Drawable[]
                                {
                                    new MapSetButton(FontAwesome6.Solid.Star, () => { }),
                                    new MapSetDownloadButton(),
                                    // new MapSetButton(FontAwesome6.Solid.EllipsisVertical, () => { })
                                }
                            }
                        }
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ColumnDimensions = new[]
                        {
                            new Dimension(),
                            new Dimension(GridSizeMode.Absolute, 20),
                            new Dimension(GridSizeMode.Absolute, 320)
                        },
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize)
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                new FluXisSpriteText
                                {
                                    Text = "leadrbord",
                                    WebFontSize = 16,
                                    Shadow = true,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre
                                },
                                Empty(),
                                new FluXisSpriteText
                                {
                                    Text = "creator anbd stats",
                                    WebFontSize = 16,
                                    Shadow = true,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void PopIn()
    {
        content.ResizeHeightTo(0).MoveToY(1)
               .ResizeHeightTo(1, 800, Easing.OutQuint)
               .MoveToY(0, 800, Easing.OutQuint);

        scroll.ScrollTo(0, false);
        scroll.FadeOut().Delay(400).FadeIn(200);
        this.FadeIn(200);
    }

    protected override void PopOut()
    {
        content.ResizeHeightTo(0, 800, Easing.OutQuint);
        this.FadeOut(200);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        Hide();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
