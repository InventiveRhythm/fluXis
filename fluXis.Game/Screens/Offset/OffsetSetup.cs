using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Offset;

public partial class OffsetSetup : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.4f;
    public override float ParallaxStrength => .1f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => .6f;
    public override float BackgroundBlur => .4f;
    public override bool AllowMusicControl => false;

    [Resolved]
    private AudioClock clock { get; set; }

    private Bindable<float> offset;

    private Track track;
    private double time => (track?.CurrentTime ?? 0) - offset.Value - 50; // the track has an offset of 50ms before the first beat

    private Line line;
    private Container container;
    private FluXisSpriteText offsetText;

    [BackgroundDependencyLoader]
    private void load(ITrackStore tracks, FluXisConfig config)
    {
        offset = config.GetBindable<float>(FluXisSetting.GlobalOffset);
        track = tracks.Get("Menu/Offset.mp3");
        track.Looping = true;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 150,
                FontSize = 72,
                Text = "Offset Setup"
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Width = .8f,
                Height = 200,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .7f
                    },
                    new Line
                    {
                        Width = 4,
                        Height = 30,
                        Alpha = .5f,
                        X = -.25f
                    },
                    new Line
                    {
                        Width = 4,
                        Height = 30,
                        Alpha = .5f,
                        X = .25f
                    },
                    new Line
                    {
                        Width = 4,
                        Height = 160,
                        Colour = FluXisColors.Accent
                    },
                    line = new Line
                    {
                        Width = 6,
                        Height = 120,
                        Colour = FluXisColors.Accent4
                    },
                    container = new Container { RelativeSizeAxes = Axes.Both }
                }
            },
            new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 200,
                FontSize = 28,
                Text = "Current Offset:"
            },
            offsetText = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 226,
                FontSize = 44,
                Text = offset.Value.ToString("N0") + "ms"
            },
            new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 270,
                FontSize = 28,
                Text = "Use the arrow keys to adjust the offset."
            },
            new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                Y = 296,
                FontSize = 28,
                Text = "Press space to add a line."
            }
        };
    }

    protected override void LoadComplete()
    {
        offset.ValueChanged += updateOffset;
    }

    private void updateOffset(ValueChangedEvent<float> e) => offsetText.Text = e.NewValue.ToString("N0") + "ms";

    private const float bpm = 100f;
    private const float beat_length = 60000f / bpm;
    private const float fourth_beat = beat_length * 4;

    private float timeSinceLastBeat => (float)(time % fourth_beat);
    private float progress => timeSinceLastBeat / fourth_beat;

    protected override void Update()
    {
        line.X = progress < .5f ? progress : progress - 1f;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        track?.Start();
        clock.Stop();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        track?.Stop();
        clock.Start();
        return base.OnExiting(e);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.Space:
            {
                var l = new Line
                {
                    X = progress < .5f ? progress : progress - 1f,
                    Width = 4,
                    Height = 100,
                    Colour = FluXisColors.Accent2
                };

                container.Add(l);
                l.ScaleTo(new Vector2(1, 0)).ScaleTo(Vector2.One, 300, Easing.OutQuint).Delay(400).FadeOut(500).Expire();

                return true;
            }

            case Key.Left:
            {
                offset.Value -= 1;
                return true;
            }

            case Key.Right:
            {
                offset.Value += 1;
                return true;
            }

            default:
                return false;
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back) return false;

        this.Exit();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override void Dispose(bool isDisposing)
    {
        track?.Dispose();
        offset.ValueChanged -= updateOffset;
        base.Dispose(isDisposing);
    }

    private partial class Line : CircularContainer
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RelativePositionAxes = Axes.X;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Masking = true;
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both
            };
        }
    }
}
