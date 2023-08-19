using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens;

public partial class WorkInProgressScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 2f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => .7f;
    public override float BackgroundBlur => .4f;

    [Resolved]
    private AudioClock audioClock { get; set; }

    protected virtual string Title => "Work in Progress";

    private FillFlowContainer flowContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = flowContainer = new FillFlowContainer
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
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);
        flowContainer.ScaleTo(.8f).ScaleTo(1, 1000, Easing.OutElastic);
        audioClock.LowPassFilter.CutoffTo(LowPassFilter.MIN, 200);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.FadeOutFromOne(400);
        flowContainer.ScaleTo(.8f, 600);
        audioClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, 200);

        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        if (e.Action != FluXisKeybind.Back) return false;

        this.Exit();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
}
