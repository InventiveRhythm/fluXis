using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Volume;

public partial class VolumeOverlay : Container
{
    private const int max_inactive = 2000;

    private AudioManager audioManager;
    private int index;
    private int timeInactive = 1000;

    private FillFlowContainer categories;

    public VolumeOverlay()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Margin = new MarginPadding { Bottom = 50 };
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager manager)
    {
        audioManager = manager;

        Add(categories = new FillFlowContainer
        {
            Spacing = new Vector2(30, 0),
            Alpha = 0,
            Direction = FillDirection.Horizontal,
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new[]
            {
                new VolumeCategory("master", manager.Volume),
                new VolumeCategory("music", manager.VolumeTrack),
                new VolumeCategory("effect", manager.VolumeSample)
            }
        });

        changeCategory(0);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.AltPressed)
        {
            switch (e.Key)
            {
                case Key.Up:
                    changeVolume(.01f);
                    return true;

                case Key.Down:
                    changeVolume(-.01f);
                    return true;

                case Key.Left:
                    changeCategory(-1);
                    return true;

                case Key.Right:
                    changeCategory(1);
                    return true;
            }
        }

        return base.OnKeyDown(e);
    }

    private void changeVolume(float amount)
    {
        timeInactive = 0;

        switch (index)
        {
            case 0:
                audioManager.Volume.Value += amount;
                break;

            case 1:
                audioManager.VolumeTrack.Value += amount;
                break;

            case 2:
                audioManager.VolumeSample.Value += amount;
                break;
        }
    }

    private void changeCategory(int amount)
    {
        timeInactive = 0;

        index += amount;
        index = index switch
        {
            < 0 => 2,
            > 2 => 0,
            _ => index
        };

        for (var i = 0; i < categories.Children.Count; i++)
        {
            var child = (VolumeCategory)categories.Children[i];
            child.FadeTo(i == index ? 1 : .6f, 200);
        }
    }

    protected override void Update()
    {
        if (timeInactive == 0)
            categories.FadeIn(100);

        if (timeInactive > max_inactive)
            categories.FadeOut(100);
        else
            timeInactive += (int)Clock.ElapsedFrameTime;
    }
}
