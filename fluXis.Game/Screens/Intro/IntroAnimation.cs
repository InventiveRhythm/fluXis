using System.Linq;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Intro;

public partial class IntroAnimation : FluXisScreen
{
    public override float Zoom => 1.2f;
    public override float BackgroundDim => 1;
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;

    private const int bars = 8;
    private const int bars_duration = 800;
    private const int bars_delay = 200;

    private const int snap_duration = 124;
    private const int start = 57;

    private const int time_first = start + snap_duration * 0;
    private const int time_second = start + snap_duration * 1;
    private const int time_third = start + snap_duration * 2;
    private const int time_fourth = start + snap_duration * 3;

    private const int out_time = 806;
    private const int out_duration = 500;

    [Resolved]
    private FluXisGameBase game { get; set; }

    private Sample sample;

    private Container barsContainer;

    private IntroCorner topLeft;
    private IntroCorner topRight;
    private IntroCorner bottomRight;
    private IntroCorner bottomLeft;
    private Sprite logo;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, TextureStore textures)
    {
        RelativeSizeAxes = Axes.Both;
        Depth = -1;

        sample = samples.Get("Intro/startup");

        InternalChildren = new Drawable[]
        {
            barsContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Scale = new Vector2(1.2f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 10,
                ChildrenEnumerable = Enumerable.Range(0, bars).Select(i => new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    MaskingSmoothness = 0,
                    Width = 1f / bars,
                    X = i / (float)bars,
                    Height = 1.2f,
                    Colour = Colour4.Black
                })
            },
            topLeft = new IntroCorner(Corner.TopLeft),
            topRight = new IntroCorner(Corner.TopRight),
            bottomRight = new IntroCorner(Corner.BottomRight),
            bottomLeft = new IntroCorner(Corner.BottomLeft),
            logo = new Sprite
            {
                Texture = textures.Get("Logos/logo-text.png"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        game.MenuScreen.PreEnter();

        ScheduleAfterChildren(() => ScheduleAfterChildren(() =>
        {
            sample.Play();

            topLeft.Show(time_first, snap_duration);
            topRight.Show(time_second, snap_duration);
            bottomRight.Show(time_third, snap_duration);
            bottomLeft.Show(time_fourth, snap_duration);

            this.Delay(out_time).Schedule(() =>
            {
                topLeft.Hide(out_duration);
                topRight.Hide(out_duration);
                bottomRight.Hide(out_duration);
                bottomLeft.Hide(out_duration);
                logo.ScaleTo(1.1f).FadeIn(out_duration).ScaleTo(1, out_duration * 2, Easing.OutQuint);

                this.Delay(out_duration * 2).Schedule(() => this.Push(game.MenuScreen));
            });
        }));
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.Delay(bars_duration + bars_delay).FadeOut();
        Schedule(slideBars);
    }

    private void slideBars()
    {
        foreach (var bar in barsContainer.Children)
        {
            var rngDelay = RNG.Next(0, bars_delay);
            bar.Delay(rngDelay).MoveToY(-1.2f, bars_duration, Easing.OutCirc);
        }
    }
}
