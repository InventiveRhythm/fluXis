using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Volume;

public partial class VolumeOverlay : Container, IKeyBindingHandler<FluXisKeybind>
{
    private const int max_inactive = 2000;

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
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(.4f),
                Radius = 10,
                Offset = new Vector2(0, 1)
            },
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
                        new VolumeCategory
                        {
                            Text = "Master",
                            VolumeOverlay = this,
                            Bindable = audioManager.Volume
                        },
                        new VolumeCategory
                        {
                            Text = "Music",
                            VolumeOverlay = this,
                            Bindable = audioManager.VolumeTrack
                        },
                        new VolumeCategory
                        {
                            Text = "Effects",
                            VolumeOverlay = this,
                            Bindable = audioManager.VolumeSample
                        },
                        new VolumeCategory
                        {
                            Text = "Hitsounds",
                            VolumeOverlay = this,
                            Bindable = config.GetBindable<double>(FluXisSetting.HitSoundVolume)
                        }
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

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.VolumeDecrease:
                changeVolume(-.01f);
                return true;

            case FluXisKeybind.VolumeIncrease:
                changeVolume(.01f);
                return true;

            case FluXisKeybind.VolumePreviousCategory:
                changeCategory(-1);
                return true;

            case FluXisKeybind.VolumeNextCategory:
                changeCategory(1);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
}
