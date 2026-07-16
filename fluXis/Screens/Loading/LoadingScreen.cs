using System;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Intro;
using fluXis.Screens.Warning;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Loading;

public partial class LoadingScreen : FluXisScreen
{
    public override float BackgroundDim => 1;

    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private FluXisGame game { get; set; }

    private FluXisGameBase.LoadInfo loadInfo { get; }
    private FluXisSpriteText loadingText { get; }
    private Circle bar { get; }

    private DrawableTrack bgm;

    public LoadingScreen(FluXisGameBase.LoadInfo loadInfo)
    {
        this.loadInfo = loadInfo;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black
            },
            new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                BlurSigma = new Vector2(256),
                FrameBufferScale = new Vector2(.4f),
                Child = new LoadingBubbles()
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
                                Colour = Theme.Text,
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
        };
    }

    [BackgroundDependencyLoader]
    private void load(ITrackStore tracks)
    {
        var track = tracks.Get("loading-screen") ?? tracks.GetVirtual(1000);
        game.Add(bgm = new DrawableTrack(track)
        {
            Name = "Loading BGM",
            RestartPoint = 8467,
            Looping = true
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadInfo.TaskStarted += task => Scheduler.ScheduleIfNeeded(() =>
        {
            var str = $"{task.Name}";

            if (task.ShowProgress)
                str += $" ({loadInfo.TasksFinished + 1}/{loadInfo.TasksTotal})";

            loadingText.Text = str;
            bar.ResizeWidthTo(Math.Min(loadInfo.TasksFinished + 1, loadInfo.TasksTotal) / (float)loadInfo.TasksTotal, 600, Easing.OutQuint);
        });

        loadInfo.AllFinished += complete;
    }

    private void complete() => Scheduler.ScheduleIfNeeded(() =>
    {
        if (config.Get<bool>(FluXisSetting.SkipIntro))
            this.Push(new IntroAnimation());
        else
            this.Push(new WarningScreen());
    });

    public override void OnEntering(ScreenTransitionEvent e)
    {
        bgm.Start();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        bgm.VolumeTo(0, Styling.TRANSITION_FADE).Expire();
        this.FadeOut(Styling.TRANSITION_FADE);
        this.MoveToY(20, Styling.TRANSITION_MOVE, Easing.OutQuint);
        base.OnSuspending(e);
    }
}
