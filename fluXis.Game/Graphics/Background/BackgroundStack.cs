using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Toolbar;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class BackgroundStack : CompositeDrawable
{
    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private Toolbar toolbar { get; set; }

    private readonly List<Background> scheduledBackgrounds = new();
    private readonly Container backgroundContainer;
    private readonly ParallaxContainer parallaxContainer;
    private readonly BackgroundVideo backgroundVideo;
    private readonly Box swipeAnimation;
    private readonly Box backgroundDim;
    private string currentBackground;
    private float zoom = 1;
    private float blur;

    private Bindable<bool> backgroundPulse;

    public float Zoom
    {
        get => zoom;
        set
        {
            backgroundContainer.ScaleTo(value + .05f, 1000, Easing.OutQuint);
            zoom = value;
        }
    }

    public float ParallaxStrength
    {
        get => parallaxContainer.Strength;
        set => parallaxContainer.Strength = value;
    }

    public BackgroundStack()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            parallaxContainer = new ParallaxContainer
            {
                Child = backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(1.05f)
                },
                RelativeSizeAxes = Axes.Both
            },
            backgroundVideo = new BackgroundVideo(),
            backgroundDim = new Box
            {
                Colour = Colour4.Black,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            swipeAnimation = new Box
            {
                Colour = Colour4.Black,
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Width = 1.1f,
                Alpha = 0,
                Shear = new Vector2(.1f, 0),
                X = -1.1f
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        backgroundPulse = config.GetBindable<bool>(FluXisSetting.BackgroundPulse);
    }

    protected override void LoadComplete()
    {
        blur = 0.01f;
        AddBackgroundFromMap(null);
    }

    public void SetDim(float alpha, float duration = 200) => backgroundDim.FadeTo(alpha, duration);

    public void SetBlur(float blur, float duration = 200)
    {
        if (this.blur == blur)
            return;

        this.blur = blur;

        var last = backgroundContainer.Count > 0 ? backgroundContainer[^1] : null;

        if (last is Background background) // 🧀 transforming blur is laggy so we just replace the background
        {
            Schedule(() =>
            {
                backgroundContainer.Add(new Background(background.Map)
                {
                    Blur = blur,
                    Duration = duration
                });
            });
        }
    }

    protected override void Update()
    {
        // this is EXTREMELY laggy, and i do not know why
        // Padding = new MarginPadding { Top = toolbar.Height - 10 + toolbar.Y };

        while (scheduledBackgrounds.Count > 0)
        {
            var background = scheduledBackgrounds[0];
            scheduledBackgrounds.RemoveAt(0);
            backgroundContainer.Add(background);
        }

        while (backgroundContainer.Children.Count > 1)
        {
            if (backgroundContainer.Children[1].Alpha == 1)
                backgroundContainer.Remove(backgroundContainer.Children[0], true);
            else
                break;
        }

        if (backgroundPulse.Value)
        {
            var amplitude = clock.Amplitudes.Where((_, i) => i is > 0 and < 4).ToList().Average();
            Scale = new Vector2(1 + amplitude * .02f);
        }
        else if (Scale.X != 1)
            Scale = new Vector2(1);

        base.Update();
    }

    public void AddBackgroundFromMap(RealmMap map)
    {
        string file = map?.MapSet?.GetPathForFile(map.Metadata?.Background);

        if (map != null)
        {
            string path = file ?? map.Hash;

            if (path == currentBackground && !string.IsNullOrEmpty(path))
                return;

            currentBackground = path;
        }
        else currentBackground = null;

        scheduledBackgrounds.Add(new Background(map) { Blur = blur });
    }

    public void SetVideoBackground(RealmMap map, MapInfo info)
    {
        backgroundVideo.Map = map;
        backgroundVideo.Info = info;
        backgroundVideo.LoadVideo();
    }

    public void StartVideo()
    {
        backgroundVideo.Start();
    }

    public void StopVideo()
    {
        backgroundVideo.Stop();
    }

    public void SwipeAnimation()
    {
        const int duration = 1100;
        const int delay = 1200;
        const int fade_duration = 1500;

        swipeAnimation.MoveToX(-1.1f)
                      .FadeTo(.5f)
                      .MoveToX(0, duration, Easing.OutQuint)
                      .Delay(delay)
                      .FadeOut(fade_duration);
    }
}
