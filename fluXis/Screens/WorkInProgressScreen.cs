using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens;

public partial class WorkInProgressScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 2f;
    public override float BackgroundDim => .7f;
    public override float BackgroundBlur => .4f;

    protected virtual string Title => "Work in Progress";

    private AudioFilter lowPass;
    private FillFlowContainer flowContainer;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        InternalChildren = new Drawable[]
        {
            lowPass = new AudioFilter(audio.TrackMixer),
            flowContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = Title,
                        FontSize = 50,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisTextFlow
                    {
                        Text = "This screen is not yet implemented.\nPlease check back later.",
                        FontSize = 30,
                        Width = 600,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeInFromZero(FADE_DURATION);
            flowContainer.ScaleTo(1.4f).ScaleTo(1, MOVE_DURATION, Easing.OutQuint);
            lowPass.CutoffTo(AudioFilter.MIN, FADE_DURATION);
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOutFromOne(FADE_DURATION);
        flowContainer.ScaleTo(.8f, MOVE_DURATION, Easing.OutQuint);
        lowPass.CutoffTo(AudioFilter.MAX, FADE_DURATION);

        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back) return false;

        this.Exit();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
