using System;
using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Gameplay.Input;

public partial class GameplayTouchInput : Container
{
    public override bool PropagatePositionalInputSubTree => true;
    public override bool PropagateNonPositionalInputSubTree => true;

    [Resolved]
    private GameplayScreen screen { get; set; }

    private GameplayInput input => screen.Input;

    private Container mainContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(50);

        Child = mainContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Scale = new Vector2(1.1f),
            Alpha = 0,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = getInputsForKeyCount(screen.RealmMap.KeyCount)
        };
    }

    protected override bool OnTouchDown(TouchDownEvent e)
    {
        mainContainer.FadeIn(100)
                     .ScaleTo(1f, 200, Easing.OutQuint);

        return false;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        mainContainer.FadeOut(100)
                     .ScaleTo(1.1f, 200, Easing.OutQuint);

        return false;
    }

    private Drawable[] getInputsForKeyCount(int keycount)
    {
        return keycount switch
        {
            4 => new Drawable[]
            {
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Y = -160,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key4k1),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key4k1)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 220,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key4k2),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key4k2)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    X = -220,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key4k3),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key4k3)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -160,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key4k4),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key4k4)
                }
            },
            5 => new Drawable[]
            {
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Y = -320,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key5k1),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key5k1)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 220,
                    Y = -160,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key5k2),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key5k2)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    IsSpaceBar = true,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key5k3),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key5k3)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    X = -220,
                    Y = -160,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key5k4),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key5k4)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -320,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key5k5),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key5k5)
                }
            },
            6 => new Drawable[]
            {
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k1),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k1)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Y = -370,
                    X = 220,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k2),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k2)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 220,
                    Y = -150,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k3),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k3)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    X = -220,
                    Y = -150,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k4),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k4)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -370,
                    X = -220,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k5),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k5)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key6k6),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key6k6)
                }
            },
            7 => new Drawable[]
            {
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k1),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k1)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 220,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k2),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k2)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 220,
                    Y = -150,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k3),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k3)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    IsSpaceBar = true,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k4),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k4)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    X = -220,
                    Y = -150,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k5),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k5)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    X = -220,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k6),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k6)
                },
                new TouchInputArea
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -370,
                    OnPressed = () => input.PressKey(FluXisGameplayKeybind.Key7k7),
                    OnReleased = () => input.ReleaseKey(FluXisGameplayKeybind.Key7k7)
                }
            },
            _ => Array.Empty<Drawable>()
        };
    }

    private partial class TouchInputArea : Container
    {
        private Container boxContainer;

        public bool IsSpaceBar { get; init; }
        public Action OnPressed { get; init; }
        public Action OnReleased { get; init; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = IsSpaceBar ? 1300 : 200;
            Height = IsSpaceBar ? 150 : 200;
            Alpha = 0.2f;

            AddInternal(boxContainer = new Container
            {
                CornerRadius = 10,
                Masking = true,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both
                }
            });
        }

        protected override bool OnTouchDown(TouchDownEvent e)
        {
            this.FadeTo(0.4f, 100);
            boxContainer.ScaleTo(1.1f, 100, Easing.OutQuint);
            OnPressed?.Invoke();
            return false;
        }

        protected override void OnTouchUp(TouchUpEvent e)
        {
            this.FadeTo(0.2f, 200);
            boxContainer.ScaleTo(1f, 300, Easing.OutQuint);
            OnReleased?.Invoke();
        }
    }
}
