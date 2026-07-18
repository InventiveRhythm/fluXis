using fluXis.Graphics;
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
    public override float BackgroundDim => 1;

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
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "Epilepsy warning!",
                        WebFontSize = 48,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    epilepsyText = new FluXisTextFlow
                    {
                        AlwaysPresent = true,
                        AutoSizeAxes = Axes.Y,
                        Text = "This game contains flashing lights and colors that may cause discomfort and/or seizures for people with photosensitive epilepsy.",
                        TextAnchor = Anchor.TopCentre,
                        WebFontSize = 20,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Width = 800
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        backgrounds.SetDim(1f);
        next();
    }

    private void next()
    {
        step++;

        TransformSequence<FillFlowContainer> seq;

        switch (step)
        {
            case 1:
                seq = epilepsyContainer.ScaleTo(1.1f).FadeInFromZero(Styling.TRANSITION_FADE)
                                       .ScaleTo(1, Styling.TRANSITION_MOVE, Easing.Out)
                                       .Then(4000)
                                       .ScaleTo(0.9f, Styling.TRANSITION_MOVE, Easing.Out)
                                       .FadeOut(Styling.TRANSITION_FADE);
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
        this.ScaleTo(0.9f, Styling.TRANSITION_MOVE, Easing.OutQuint).FadeOut(Styling.TRANSITION_FADE);
    }
}
