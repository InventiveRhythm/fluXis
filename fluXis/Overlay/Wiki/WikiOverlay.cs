using System;
using System.Collections.Generic;
using System.Linq;
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

namespace fluXis.Overlay.Wiki;

#nullable enable

public partial class WikiOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; } = null!;

    private Bindable<string> currentPath = new(string.Empty);
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
                            Margin = new MarginPadding { Top = 50 },
                            Height = 90,
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
                                    Masking = true,
                                    CornerRadius = 10,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Theme.Background3
                                        },
                                        new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            ColumnDimensions = new[]
                                            {
                                                new Dimension(GridSizeMode.Absolute, 50),
                                                new Dimension(GridSizeMode.Absolute, 5),
                                                new Dimension(GridSizeMode.AutoSize)
                                            },
                                            Content = new[]
                                            {
                                                new Drawable[]
                                                {
                                                    new BackButton(this)
                                                    {
                                                        Margin = new MarginPadding {Left = 10},
                                                        Anchor = Anchor.Centre,
                                                        Origin = Anchor.Centre,
                                                    },
                                                    Empty(),
                                                    new WikiNav(currentPath, this)
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Anchor = Anchor.Centre,
                                                        Origin = Anchor.Centre,
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
        private readonly WikiOverlay overlay;

        public WikiNav(Bindable<string> currentPathBindable, WikiOverlay overlay)
        {
            currentPath = currentPathBindable;
            this.overlay = overlay;

            Direction = FillDirection.Horizontal;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            currentPath.BindValueChanged((path) =>
            {
                if (path.NewValue == path.OldValue) return;

                buildNav(path.NewValue);
            }, true);
        }

        private void buildNav(string newPath)
        {
            Hide();
            Clear();

            var pathNames = newPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var paths = getPaths(newPath);

            if (pathNames.First() != "home")
            {
                pathNames.Insert(0, "home");
                paths.Insert(0, "home");
            }

            foreach ((string name, string path) in pathNames.Zip(paths))
            {
                if ("/" + path == newPath || path == newPath)
                    addPathbutton(path, name, false);
                else
                    addPathbutton(path, name);
            }

            Show();
        }

        private void addPathbutton(string path, string name, bool addSeperator = true)
        {
            AddInternal(
                new PathButton(path, name, overlay.NavigateTo)
            );

            if (addSeperator)
            {
                AddInternal(
                    new Seperator()
                );
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

        private partial class Seperator : FluXisSpriteIcon
        {
            public Seperator()
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

        private HoverLayer hover = null!;
        private FlashLayer flash = null!;

        private WikiOverlay overlay;

        public BackButton(WikiOverlay overlay)
        {
            this.overlay = overlay;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(50);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 10;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome6.Solid.AngleLeft,
                    Size = new Vector2(20)
                }
            };
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

        protected override bool OnHover(HoverEvent e)
        {
            samples?.Hover();
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            overlay.NavigateBack();
            samples?.Click();
            flash.Show();
            return base.OnClick(e);
        }
    }

    private partial class PathButton : ClickableContainer
    {
        [Resolved]
        private UISamples? samples { get; set; }

        private HoverLayer hover = null!;
        private FlashLayer flash = null!;
        private Container content = null!;

        public string Path { get; private set; }
        public new string Name { get; private set; }

        private Action<string, bool> navigateAction;

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
                        hover = new HoverLayer(),
                        flash = new FlashLayer(),
                        content = new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10),
                            Child = new FluXisSpriteText()
                            {
                                Text = Name,
                                FontSize = 30,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                            }
                        }
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            navigateAction?.Invoke(Path, true);
            flash.Show();
            samples?.Click();

            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            samples?.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 1000, Easing.OutElastic);
        }
    }
}
