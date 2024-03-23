using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class GlobalBackground : CompositeDrawable
{
    [Resolved]
    private IAmplitudeProvider amplitudeProvider { get; set; }

    public RealmMap DefaultMap { get; init; }
    public float InitialBlur { get; init; } = 0.01f;
    public float InitialDim { get; init; } = 0;

    private SpriteStack<BlurableBackground> stack;
    private ParallaxContainer parallaxContainer;
    private Box swipeAnimation;
    private Box backgroundDim;
    private string currentBackground;
    private float blur;

    private Bindable<bool> backgroundPulse;

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
                this.ScaleTo(1, 500, Easing.OutQuint);
        }, true);
    }

    public void SetDim(float alpha, float duration = 200) => backgroundDim.FadeTo(alpha, duration);

    public void SetBlur(float blur, float duration = 300)
    {
        if (this.blur == blur)
            return;

        this.blur = blur;
        add(stack.Current?.Map, duration);
    }

    protected override void Update()
    {
        // this is EXTREMELY laggy, and i do not know why
        // Padding = new MarginPadding { Top = toolbar.Height - 10 + toolbar.Y };

        if (backgroundPulse.Value)
        {
            var amplitude = amplitudeProvider.Amplitudes.Where((_, i) => i is > 0 and < 4).ToList().Average();
            Scale = new Vector2(1 + amplitude * .02f);
        }

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

        add(map, 300);
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

    private void add(RealmMap map, float duration)
        => Scheduler.ScheduleIfNeeded(() => stack.Add(new BlurableBackground(map, blur, duration), duration));
}
