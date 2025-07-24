using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Online.API.Requests.Leaderboards;
using fluXis.Online.Fluxel;
using fluXis.Screens.Ranking.UI;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Ranking;

public partial class Rankings : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.4f;
    public override float BackgroundDim => .8f;
    public override float BackgroundBlur => .5f;
    public override bool AutoPlayNext => true;

    [Resolved]
    private IAPIClient api { get; set; }

    private FillFlowContainer flow;
    private CornerButton backButton;
    private LoadingIcon loadingIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 100),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 50)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = new Drawable[]
                                {
                                    new FluXisSpriteIcon
                                    {
                                        Size = new Vector2(50),
                                        Icon = FontAwesome6.Solid.EarthAmericas,
                                        Shadow = true,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Spacing = new Vector2(-5),
                                        Children = new Drawable[]
                                        {
                                            new FluXisSpriteText
                                            {
                                                Text = "Global",
                                                Shadow = true,
                                                FontSize = 36
                                            },
                                            new FluXisSpriteText
                                            {
                                                Text = "Overall Rating",
                                                Shadow = true,
                                                Alpha = .8f
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            Child = new FluXisScrollContainer
                            {
                                Size = new Vector2(1200, 800),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                ScrollbarVisible = false,
                                Masking = false,
                                Child = new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 10),
                                    Children = new Drawable[]
                                    {
                                        flow = new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0, 10),
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                        },
                                        /*new FluXisButton
                                        {
                                            Width = 200,
                                            Height = 50,
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                            Text = "Load More"
                                        }*/
                                    }
                                }
                            }
                        }
                    },
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Theme.Background2
                                },
                                backButton = new CornerButton
                                {
                                    Corner = Corner.BottomLeft,
                                    ButtonText = LocalizationStrings.General.Back,
                                    Icon = FontAwesome6.Solid.AngleLeft,
                                    Action = this.Exit
                                }
                            }
                        }
                    }
                }
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadingIcon.Show();

        var req = new OverallRatingLeaderboardRequest();
        req.Success += res =>
        {
            foreach (var user in res.Data)
                flow.Add(new LeaderboardUser(user));

            loadingIcon.Hide();
        };
        req.Failure += e =>
        {
            loadingIcon.Hide();
            flow.Add(new FluXisSpriteText
            {
                Text = e.Message
            });
        };

        api.PerformRequestAsync(req);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            this.FadeInFromZero(Styling.TRANSITION_FADE);
            backButton.Show();
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        backButton.Hide();
        this.FadeOut(Styling.TRANSITION_FADE);
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;

            default:
                return false;
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
