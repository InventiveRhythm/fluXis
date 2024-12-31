using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Volume;

public partial class VolumeOverlay : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private const int max_inactive = 2000;

    [Resolved]
    private UISamples samples { get; set; }

    private AudioManager audioManager;
    private int index;
    private int timeInactive = max_inactive;

    private readonly BindableBool visible = new();

    private Container content;
    private FillFlowContainer<VolumeCategory> categories;

    public VolumeOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(20);
        AlwaysPresent = true;
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager manager, FluXisConfig config)
    {
        audioManager = manager;

        Add(content = new Container
        {
            AutoSizeAxes = Axes.Y,
            Width = 300,
            X = -400,
            CornerRadius = 10,
            Masking = true,
            EdgeEffect = FluXisStyles.ShadowMedium,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                categories = new FillFlowContainer<VolumeCategory>
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(0, 10),
                    Direction = FillDirection.Vertical,
                    Children = new[]
                    {
                        new VolumeCategory("Master", audioManager.Volume, this),
                        new VolumeCategory("Music", audioManager.VolumeTrack, this),
                        new VolumeCategory("Effects", audioManager.VolumeSample, this),
                        new VolumeCategory("Hitsounds", config.GetBindable<double>(FluXisSetting.HitSoundVolume), this),
                    }
                }
            }
        });

        visible.BindValueChanged(updateVisibility, true);
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (e.AltPressed)
        {
            changeVolume(e.ScrollDelta.Y * .01f);
            return true;
        }

        return false;
    }

    private void changeVolume(float amount)
    {
        timeInactive = 0;

        var category = categories.Children[index];
        category.Bindable.Value += amount;
    }

    private void changeCategory(int amount)
    {
        timeInactive = 0;

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
        timeInactive = 0;
        int delta = categories.IndexOf(cat) - index;
        changeCategory(delta);
        samples.Hover();
    }

    protected override void Update()
    {
        timeInactive += (int)Clock.ElapsedFrameTime;
        visible.Value = timeInactive < max_inactive;
    }

    private void updateVisibility(ValueChangedEvent<bool> e)
    {
        if (e.OldValue == e.NewValue)
            return;

        if (e.NewValue)
        {
            content.MoveToX(0, 500, Easing.OutQuint);
            changeCategory(-index);
        }
        else
            content.MoveToX(-400, 500, Easing.InQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.VolumeDecrease:
                changeVolume(-.01f);
                return true;

            case FluXisGlobalKeybind.VolumeIncrease:
                changeVolume(.01f);
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
