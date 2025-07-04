using System.Linq;
using System.Threading;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Screens;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Graphics.Background;

public partial class GlobalBackground : CompositeDrawable
{
    [Resolved]
    private IAmplitudeProvider amplitudeProvider { get; set; }

    public RealmMap DefaultMap { get; init; }
    public float InitialBlur { get; init; } = 0.01f;
    public float InitialDim { get; init; }

    private SpriteStack<BlurableBackground> stack;
    private ParallaxContainer parallaxContainer;
    private Box swipeAnimation;
    private Box backgroundDim;
    private string currentBackground;
    private float blur;

    private Bindable<bool> backgroundPulse;

    [CanBeNull]
    private CancellationTokenSource cancellationSource;

    public float Zoom { set => stack.ScaleTo(value, 1000, Easing.OutQuint); }
    public float ParallaxStrength { set => parallaxContainer.Strength = value; }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        backgroundPulse = config.GetBindable<bool>(FluXisSetting.BackgroundPulse);

        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            parallaxContainer = new ParallaxContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = stack = new SpriteStack<BlurableBackground>
                {
                    AutoFill = false,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            },
            backgroundDim = new Box
            {
                Colour = Colour4.Black,
                RelativeSizeAxes = Axes.Both,
                Alpha = InitialDim
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

    protected override void LoadComplete()
    {
        blur = InitialBlur;
        AddBackgroundFromMap(DefaultMap);

        backgroundPulse.BindValueChanged(e =>
        {
            if (!e.NewValue)
                this.ScaleTo(1, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        }, true);
    }

    public void SetDim(float alpha) => backgroundDim.FadeTo(alpha, FluXisScreen.FADE_DURATION);

    public void SetBlur(float blur)
    {
        if (this.blur == blur)
            return;

        this.blur = blur;
        addMap(stack.Current?.Map);
    }

    protected override void Update()
    {
        // this is EXTREMELY laggy, and I do not know why
        // Padding = new MarginPadding { Top = toolbar.Height - 10 + toolbar.Y };

        if (backgroundPulse.Value)
        {
            var amplitude = amplitudeProvider.Amplitudes.Where((_, i) => i is > 0 and < 4).ToList().Average();
            Scale = new Vector2(1 + amplitude * .02f);
        }

        base.Update();
    }

    public void PushBackground(BlurableBackground background) => Scheduler.ScheduleIfNeeded(() =>
    {
        cancellationSource?.Cancel();
        LoadComponentAsync(background, b =>
        {
            b.FadeInFromZero(FluXisScreen.FADE_DURATION);
            stack.Add(b);
        }, (cancellationSource = new CancellationTokenSource()).Token);
    });

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

        addMap(map);
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

    private void addMap(RealmMap map) => PushBackground(new BlurableBackground(map, blur));
}
