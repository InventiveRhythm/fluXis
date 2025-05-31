using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Screens.Intro;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Warning;

public partial class WarningScreen : FluXisScreen
{
    public override bool ShowToolbar => false;

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    private int step;

    private FillFlowContainer epilepsyContainer;
    private FluXisTextFlow epilepsyText;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            epilepsyContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
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
                        AlwaysPresent = true,
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
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        backgrounds.SetDim(1f, 0);

        next();

        // yes, this is stupid, and I know
        this.Delay(200).FadeIn().OnComplete(_ => epilepsyContainer.AutoSizeDuration = 99999);
    }

    private void next()
    {
        step++;

        TransformSequence<FillFlowContainer> seq;

        switch (step)
        {
            case 1:
                seq = epilepsyContainer.ScaleTo(1.1f).FadeInFromZero(FADE_DURATION)
                                       .ScaleTo(1, MOVE_DURATION, Easing.OutQuint)
                                       .Then(5000)
                                       .ScaleTo(0.9f, MOVE_DURATION, Easing.OutQuint)
                                       .FadeOut(FADE_DURATION);

                epilepsyText.Delay(2000).ScaleTo(1.1f)
                            .FadeIn(FADE_DURATION)
                            .ScaleTo(1, MOVE_DURATION, Easing.OutQuint);
                break;

            default:
                continueToMenu();
                return;
        }

        seq.OnComplete(_ => next());
    }

    protected override bool OnClick(ClickEvent e)
    {
        continueToMenu();
        return true;
    }

    private void continueToMenu()
    {
        this.Push(new IntroAnimation());
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.ScaleTo(0.9f, MOVE_DURATION, Easing.OutQuint).FadeOut(FADE_DURATION);
    }
}
