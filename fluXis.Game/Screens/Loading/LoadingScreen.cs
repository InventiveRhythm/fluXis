using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
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
                    loadingText = new FluXisSpriteText
                    {
                        Text = "Loading...",
                        FontSize = 28,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new LoadingIcon
                    {
                        Size = new Vector2(50),
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight
                    }
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadInfo.TaskStarted += task => loadingText.Text = $"Loading... {task} ({loadInfo.TasksDone + 1}/{loadInfo.TotalTasks})";
        loadInfo.AllFinished += complete;
    }

    private void complete()
    {
        if (config.Get<bool>(FluXisSetting.SkipIntro))
            this.Push(new IntroAnimation());
        else
            this.Push(new WarningScreen());
    }
}
