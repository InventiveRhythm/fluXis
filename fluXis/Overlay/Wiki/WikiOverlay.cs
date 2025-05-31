using System.Collections.Generic;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Containers.Markdown;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Online.API.Requests.Wiki;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using Markdig.Syntax.Inlines;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Wiki;

#nullable enable

public partial class WikiOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; } = null!;

    private string currentPath = string.Empty;
    private Stack<string> history = new();

    private Container content = null!;
    private FluXisScrollContainer scroll = null!;
    private OnlineErrorContainer error = null!;
    private LoadingIcon loading = null!;

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
                Width = 1536,
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
                            Colour = FluXisColors.Background2
                        },
                        scroll = new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false
                        },
                        error = new OnlineErrorContainer
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        loading = new LoadingIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(50),
                            Alpha = 0
                        }
                    }
                }
            }
        };
    }

    public void NavigateTo(string path, bool keepHistory = false)
    {
        Show();

        if (path == currentPath)
            return;

        if (!keepHistory)
            history.Clear();

        currentPath = path;

        loading.FadeIn(200);
        var req = new WikiPageRequest(path);
        req.Success += () =>
        {
            loading.FadeOut(200);
            error.Hide();
            scroll.Clear();

            var contents = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(16),
                Children = new Drawable[]
                {
                    new ForcedHeightText
                    {
                        Text = "Contents",
                        WebFontSize = 14,
                        Height = 20,
                        Alpha = .8f,
                        Margin = new MarginPadding { Bottom = 4 },
                    }
                }
            };

            var md = new FluXisMarkdown
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                DocumentMargin = new MarginPadding(),
                DocumentPadding = new MarginPadding(24),
                HeadingCreated = h =>
                {
                    var text = h.Inline?.FirstChild as LiteralInline;

                    switch (h.Level)
                    {
                        case 2:
                            contents.Add(new ForcedHeightText(true)
                            {
                                Text = text?.ToString() ?? string.Empty,
                                RelativeSizeAxes = Axes.X,
                                WebFontSize = 14,
                                Height = 20,
                            });
                            break;

                        case 3:
                            contents.Add(new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Padding = new MarginPadding { Left = 16 },
                                Child = new ForcedHeightText(true)
                                {
                                    Text = text?.ToString() ?? string.Empty,
                                    RelativeSizeAxes = Axes.X,
                                    WebFontSize = 14,
                                    Height = 20,
                                }
                            });
                            break;
                    }
                },
                OnLinkClicked = l =>
                {
                    if (l.StartsWith("/wiki"))
                        l = l[5..];

                    history.Push(currentPath);
                    NavigateTo(l, true);
                }
            };
            md.Text = req.ResponseString;

            scroll.Add(new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(12) { Top = 50 + 12 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, 240),
                        new Dimension(GridSizeMode.Absolute, 8),
                        new Dimension(),
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize),
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                CornerRadius = 8,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = FluXisColors.Background3
                                    },
                                    contents
                                }
                            },
                            Empty(),
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                CornerRadius = 8,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = FluXisColors.Background3
                                    },
                                    md
                                }
                            }
                        }
                    }
                }
            });
        };

        api.PerformRequestAsync(req);
    }

    public bool NavigateBack()
    {
        if (history.Count == 0)
            return false;

        NavigateTo(history.Pop(), true);
        return true;
    }

    protected override void PopIn()
    {
        if (currentPath == string.Empty)
            NavigateTo("/home");

        content.ResizeHeightTo(0).MoveToY(1)
               .ResizeHeightTo(1, 800, Easing.OutQuint)
               .MoveToY(0, 800, Easing.OutQuint);

        scroll.ScrollToStart();
        scroll.FadeOut().Delay(400).FadeIn(200);
        this.FadeIn(200);
    }

    protected override void PopOut()
    {
        error.Hide();
        content.ResizeHeightTo(0, 800, Easing.OutQuint);
        this.FadeOut(200);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        if (NavigateBack())
            return true;

        Hide();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
