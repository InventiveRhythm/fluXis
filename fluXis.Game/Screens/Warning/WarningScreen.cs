using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Screens.Intro;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Warning;

public partial class WarningScreen : FluXisScreen
{
    public override bool ShowToolbar => false;

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    private FillFlowContainer epilepsyContainer;
    private FluXisTextFlow epilepsyText;

    private FillFlowContainer earlyAccessContainer;
    private FluXisTextFlow earlyAccessText;

    private bool shouldSkip;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        if (config.Get<bool>(FluXisSetting.SkipIntro))
        {
            shouldSkip = true;
            return;
        }

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black
            },
            epilepsyContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                AutoSizeDuration = 500,
                AutoSizeEasing = Easing.OutQuint,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 20),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "Epilepsy warning!",
                        FontSize = 60,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    epilepsyText = new FluXisTextFlow
                    {
                        AutoSizeAxes = Axes.Y,
                        Text = "This game contains flashing lights and colors that may cause discomfort and/or seizures for people with photosensitive epilepsy.",
                        TextAnchor = Anchor.TopCentre,
                        FontSize = 30,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Width = 800,
                        Alpha = 0
                    }
                }
            },
            earlyAccessContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                AutoSizeDuration = 500,
                AutoSizeEasing = Easing.OutQuint,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Spacing = new Vector2(0, 20),
                Alpha = 0,
                Y = 20,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "This game is currently in early access.",
                        FontSize = 60,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    earlyAccessText = new FluXisTextFlow
                    {
                        AutoSizeAxes = Axes.Y,
                        Text = "This means that the game is not finished yet and that you may encounter bugs and other issues.\n\nIf you encounter any issues, please report them on the GitHub repository or the Discord server.\n",
                        TextAnchor = Anchor.TopCentre,
                        FontSize = 30,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Width = 1400,
                        Alpha = 0
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        backgrounds.SetDim(1f, 0);

        if (shouldSkip)
        {
            continueToMenu();
            return;
        }

        epilepsyContainer.FadeInFromZero(400)
                         .Then(5000).MoveToY(-20, 800, Easing.OutQuint).FadeOut(400);

        epilepsyText.Delay(2000).FadeIn(400);

        earlyAccessContainer.Delay(6000).FadeInFromZero(400).MoveToY(0, 800, Easing.OutQuint)
                            .Then(5000).MoveToY(-20, 800, Easing.OutQuint).FadeOut(400).OnComplete(_ => continueToMenu());

        earlyAccessText.Delay(7000).FadeIn(400);
    }

    private void continueToMenu()
    {
        this.Push(new IntroAnimation());
    }
}
