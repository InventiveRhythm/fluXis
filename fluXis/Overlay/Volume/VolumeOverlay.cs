using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Input;
using fluXis.Screens;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Volume;

public partial class VolumeOverlay : VisibilityContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override bool PropagateNonPositionalInputSubTree => true;
    public override bool PropagatePositionalInputSubTree => true;
    protected override bool StartHidden => true;

    private const int max_inactive = 2000;

    [Resolved]
    private UISamples samples { get; set; }

    private AudioManager audioManager;
    private int index;
    private int timeInactive = max_inactive;

    private FillFlowContainer<VolumeCategory> categories;

    [BackgroundDependencyLoader]
    private void load(AudioManager manager, FluXisConfig config)
    {
        audioManager = manager;

        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Height = .6f,
                Colour = ColourInfo.GradientVertical(Colour4.Black.Opacity(0), Colour4.Black.Opacity(.6f)),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            },
            categories = new FillFlowContainer<VolumeCategory>
            {
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(32),
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -100,
                Children = new[]
                {
                    new VolumeCategory("Master", audioManager.Volume, this),
                    new VolumeCategory("Music", audioManager.VolumeTrack, this),
                    new VolumeCategory("Effects", audioManager.VolumeSample, this),
                    new VolumeCategory("Hitsounds", config.GetBindable<double>(FluXisSetting.HitSoundVolume), this),
                }
            }
        };
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (e.AltPressed)
        {
            changeVolume(e.ScrollDelta.Y * .02f);
            return true;
        }

        return false;
    }

    public void ResetInactiveTimer()
    {
        Show();
        timeInactive = 0;
    }

    private void changeVolume(float amount)
    {
        ResetInactiveTimer();

        var category = categories.Children[index];
        category.Bindable.Value += amount;
    }

    private void changeCategory(int amount)
    {
        ResetInactiveTimer();

        index += amount;

        if (index < 0)
            index = categories.Children.Count - 1;
        else if (index >= categories.Children.Count)
            index = 0;

        foreach (var category in categories)
            category.UpdateSelected(category == categories.Children[index]);
    }

    public void SelectCategory(VolumeCategory cat)
    {
        ResetInactiveTimer();
        int delta = categories.IndexOf(cat) - index;
        changeCategory(delta);
        samples.Hover();
    }

    protected override void Update()
    {
        timeInactive += (int)Clock.ElapsedFrameTime;

        if (timeInactive >= max_inactive)
            Hide();
        else
            Show();
    }

    protected override void PopIn()
    {
        this.FadeIn(FluXisScreen.FADE_DURATION);
        categories.MoveToY(-60).MoveToY(-100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        changeCategory(-index);
    }

    protected override void PopOut()
    {
        this.FadeOut(FluXisScreen.FADE_DURATION);
        categories.MoveToY(200, FluXisScreen.MOVE_DURATION, Easing.InQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.VolumeDecrease:
                changeVolume(-.02f);
                return true;

            case FluXisGlobalKeybind.VolumeIncrease:
                changeVolume(.02f);
                return true;

            case FluXisGlobalKeybind.VolumePreviousCategory:
                changeCategory(-1);
                return true;

            case FluXisGlobalKeybind.VolumeNextCategory:
                changeCategory(1);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
