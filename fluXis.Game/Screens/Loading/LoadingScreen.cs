using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Intro;
using fluXis.Game.Screens.Warning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Loading;

public partial class LoadingScreen : FluXisScreen
{
    public override float BackgroundDim => 1;

    [Resolved]
    private FluXisConfig config { get; set; }

    private FluXisGame.LoadInfo loadInfo { get; }
    private FluXisSpriteText loadingText { get; }
    private Circle bar;

    public LoadingScreen(FluXisGame.LoadInfo loadInfo)
    {
        this.loadInfo = loadInfo;

        AddRangeInternal(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Padding = new MarginPadding(30),
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Spacing = new Vector2(6),
                        Children = new Drawable[]
                        {
                            loadingText = new FluXisSpriteText
                            {
                                Text = "Loading...",
                                WebFontSize = 16
                            },
                            new CircularContainer
                            {
                                Size = new Vector2(240, 8),
                                Masking = true,
                                Colour = FluXisColors.Text,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Alpha = .2f
                                    },
                                    bar = new Circle
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0,
                                    }
                                }
                            }
                        }
                    },
                    new LoadingIcon
                    {
                        Size = new Vector2(32),
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight
                    }
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadInfo.TaskStarted += task =>
        {
            loadingText.Text = $"Loading {task}... ({loadInfo.TasksDone + 1}/{loadInfo.TotalTasks})";
            bar.ResizeWidthTo((loadInfo.TasksDone + 1) / (float)loadInfo.TotalTasks, 300, Easing.OutQuint);
        };

        loadInfo.AllFinished += complete;
    }

    private void complete()
    {
        if (config.Get<bool>(FluXisSetting.SkipIntro))
            this.Push(new IntroAnimation());
        else
            this.Push(new WarningScreen());
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(FADE_DURATION);
        base.OnSuspending(e);
    }
}
