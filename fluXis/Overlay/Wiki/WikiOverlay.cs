using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Containers.Markdown;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Input;
using fluXis.Online.API.Requests.Wiki;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using Markdig.Syntax.Inlines;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osu.Framework.Logging;

namespace fluXis.Overlay.Wiki;

#nullable enable

public partial class WikiOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; } = null!;

    private Bindable<string> currentPath = new(string.Empty);
    private Bindable<string> currentHeading = new(string.Empty);
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
                    EdgeEffect = Styling.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background2
                        },
                        scroll = new FluXisScrollContainer
                        {
                            Margin = new MarginPadding {Top = 80},
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
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Margin = new MarginPadding { Top = 60 },
                            Height = 75,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background2
                                },
                                new Container
                                {
                                    Padding = new MarginPadding(10),
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    
                                    Children = new Drawable[]
                                    {
                                        new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            ColumnDimensions = new[]
                                            {
                                                new Dimension(GridSizeMode.Absolute, 50),
                                                new Dimension(GridSizeMode.Absolute, 15),
                                                new Dimension(GridSizeMode.Absolute, 1536 - 50 - 15)
                                            },
                                            Content = new[]
                                            {
                                                new Drawable[]
                                                {
                                                    new Container
                                                    {
                                                        Masking = true,
                                                        CornerRadius = 8,
                                                        RelativeSizeAxes = Axes.Both,
                                                        Margin = new MarginPadding {Left = 5},
                                                        Children = new Drawable[]
                                                        {
                                                            new Box
                                                            {
                                                                Anchor = Anchor.TopLeft,
                                                                Origin = Anchor.TopLeft,
                                                                RelativeSizeAxes = Axes.Both,
                                                                Colour = Theme.Background3
                                                            },
                                                            new BackButton(this)
                                                            {
                                                                Anchor = Anchor.Centre,
                                                                Origin = Anchor.Centre,
                                                            },
                                                        }
                                                    },
                                                    Empty(),
                                                    new Container
                                                    {
                                                        Masking = true,
                                                        CornerRadius = 8,
                                                        RelativeSizeAxes = Axes.Both,
                                                        // Padding = new MarginPadding { Right = 20 },
                                                        Width = 0.987f, // for some reason padding screws up the corners
                                                        Children = new Drawable[]
                                                        {
                                                            new Box
                                                            {
                                                                Anchor = Anchor.TopLeft,
                                                                Origin = Anchor.TopLeft,
                                                                RelativeSizeAxes = Axes.Both,
                                                                Colour = Theme.Background3
                                                            },
                                                            new WikiNav(currentPath, currentHeading, this)
                                                            {
                                                                AutoSizeAxes = Axes.Both,
                                                                Anchor = Anchor.CentreLeft,
                                                                Origin = Anchor.CentreLeft,
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public void NavigateTo(string path, bool keepHistory = false)
    {
        Show();

        if (path == currentPath.Value)
            return;

        if (!keepHistory)
            history.Clear();

        currentPath.Value = path;

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
                        case 1:
                            currentHeading.Value = text?.ToString() ?? string.Empty;
                            break;

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

                    history.Push(currentPath.Value);
                    NavigateTo(l, true);
                },
                Text = req.ResponseString
            };

            scroll.Add(new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(12) { Top = 50 + 12, Bottom = 100 },
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
                                        Colour = Theme.Background3
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
                                        Colour = Theme.Background3
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
        if (string.IsNullOrEmpty(currentPath.Value))
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

    private partial class WikiNav : FillFlowContainer
    {
        private Bindable<string> currentPath { get; init; }
        private Bindable<string> currentHeading { get; init; }
        private readonly WikiOverlay overlay;
        private static readonly char[] separator_char = new[] { '/' };

        public WikiNav(Bindable<string> currentPathBindable, Bindable<string> currentHeadingBindable, WikiOverlay overlay)
        {
            currentPath = currentPathBindable;
            currentHeading = currentHeadingBindable;
            this.overlay = overlay;

            Direction = FillDirection.Horizontal;
        }

        [BackgroundDependencyLoader]
        private void load()
        {   
            currentHeading.BindValueChanged((heading) =>
            {
                if (heading.NewValue == heading.OldValue) return;

                buildNav(currentPath.Value);
            }, true);
        }

        private void buildNav(string newPath)
        {
            ScheduleAfterChildren(() =>
            {
                Alpha = 0;
                Clear();
                var pathButtons = createPathButtons(newPath);
                AddRange(pathButtons);
                this.FadeIn(100);
            });
        }

        private List<Drawable> createPathButtons(string newPath)
        {
            var pathNames = newPath.Split(separator_char, StringSplitOptions.RemoveEmptyEntries).ToList();
            var paths = getPaths(newPath);

            if (pathNames.FirstOrDefault() != "home")
            {
                pathNames.Insert(0, "home");
                paths.Insert(0, "home");
            }

            var pathButtons = new List<Drawable>();

            foreach ((string name, string path) in pathNames.Zip(paths))
            {
                if ("/" + path == newPath || path == newPath)
                    addPathButton(path, overlay.currentHeading.Value, pathButtons, false);
                else
                    addPathButton(path, name.Humanize(LetterCasing.Title), pathButtons);
            }

            return pathButtons;
        }

        private void addPathButton(string path, string name, List<Drawable> list, bool addSeparator = true)
        {
            list.Add(new PathButton(path, name, overlay.NavigateTo));

            if (addSeparator)
            {
                list.Add(new Separator());
            }
        }
        
        private List<string> getPaths(string path)
        {
            var pathNames = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var paths = new List<string>();

            if (pathNames.Length == 0)
                return paths;

            string currentPath = pathNames[0];
            paths.Add(currentPath);

            for (int i = 1; i < pathNames.Length; i++)
            {
                currentPath += "/" + pathNames[i];
                paths.Add(currentPath);
            }

            return paths;
        }

        private partial class Separator : FluXisSpriteIcon
        {
            public Separator()
            {
                Icon = FontAwesome6.Solid.AngleRight;
                Size = new Vector2(15);
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;
            }
        }
    }

    private partial class BackButton : ClickableContainer
    {
        [Resolved]
        private UISamples? samples { get; set; }

        private readonly WikiOverlay overlay;
        
        protected HoverLayer Hover = null!;
        protected FlashLayer Flash = null!; 

        public BackButton(WikiOverlay overlay)
        {
            this.overlay = overlay;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(46);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 10;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                Hover = new HoverLayer(),
                Flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome6.Solid.AngleLeft,
                    Size = new Vector2(18)
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples?.Hover();
            Hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            Hover.Hide();
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }

        protected override bool OnClick(ClickEvent e)
        {
            overlay.NavigateBack();
            Flash.Show();
            return base.OnClick(e);
        }
    }

    private partial class PathButton : ClickableContainer
    {
        [Resolved]
        private UISamples? samples { get; set; }

        public string Path { get; private set; }
        public new string Name { get; private set; }

        private readonly Action<string, bool> navigateAction;

        private FluXisSpriteText text = null!;

        public PathButton(string path, string name, Action<string, bool> navigateAction)
        {
            Path = path;
            Name = name;
            this.navigateAction = navigateAction;

            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    CornerRadius = 5,
                    Masking = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Margin = new MarginPadding(10),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10),
                            Child = text = new FluXisSpriteText()
                            {
                                Text = Name,
                                FontSize = 30,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }
                        }
                    }
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples?.Hover();
            text.FadeColour(Theme.Highlight, 200, Easing.Out);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            text.FadeColour(Theme.Text, 200, Easing.Out);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            navigateAction?.Invoke(Path, true);
            return base.OnClick(e);
        }
    }
}
