using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Intro;

public partial class IntroScreen : FluXisScreen
{
    public override bool ShowToolbar => false;

    [Resolved]
    private Toolbar toolbar { get; set; }

    private FillFlowContainer epilepsyContainer;
    private bool shouldSkip;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        if (config.GetBindable<bool>(FluXisSetting.SkipIntro).Value)
        {
            shouldSkip = true;
            return;
        }

        InternalChild = epilepsyContainer = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Scale = new Vector2(0.9f),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Epilepsy warning!",
                    FontSize = 60,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Margin = new MarginPadding { Bottom = 20 }
                },
                new FluXisTextFlow
                {
                    Text = "This game contains flashing lights and colors that may cause discomfort and/or seizures for people with photosensitive epilepsy.",
                    FontSize = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Width = 800,
                    TextAnchor = Anchor.TopCentre
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        if (shouldSkip)
        {
            continueToMenu();
            return;
        }

        const int scale_duration = 700;
        const int fade_duration = 500;

        epilepsyContainer.FadeInFromZero(fade_duration).ScaleTo(1f, scale_duration, Easing.OutQuint)
                         .Then(6000).ScaleTo(1.1f, scale_duration, Easing.OutQuint).FadeOut(fade_duration)
                         .OnComplete(_ => continueToMenu());
    }

    private void continueToMenu()
    {
        var screen = new MenuScreen();
        toolbar.MenuScreen = screen;
        this.Push(screen);
    }
}
