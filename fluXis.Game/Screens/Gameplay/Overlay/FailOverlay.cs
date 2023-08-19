using fluXis.Game.Graphics.Patterns;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Screens.Gameplay.Overlay.Fail;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay;

public partial class FailOverlay : Container, IKeyBindingHandler<FluXisKeybind>
{
    public GameplayScreen Screen { get; set; }

    private readonly StripePattern pattern;
    private readonly FluXisSpriteText text;
    private readonly Box wedgeLeft;
    private readonly Box wedgeRight;
    private readonly FailResults results;

    public FailOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Children = new Drawable[]
        {
            wedgeLeft = new Box
            {
                RelativePositionAxes = Axes.Both,
                RelativeSizeAxes = Axes.Both,
                Width = 1.1f,
                X = -1.1f,
                Shear = new Vector2(.1f, 0),
                Colour = Colour4.FromHex("#090912")
            },
            wedgeRight = new Box
            {
                RelativePositionAxes = Axes.Both,
                RelativeSizeAxes = Axes.Both,
                Width = 1.1f,
                X = 1.1f,
                Shear = new Vector2(.1f, 0),
                Colour = Colour4.FromHex("#090912")
            },
            pattern = new StripePattern
            {
                Speed = new Vector2(-300)
            },
            text = new FluXisSpriteText
            {
                Text = "FAILED",
                FontSize = 100,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            results = new FailResults
            {
                FailOverlay = this
            }
        };
    }

    public override void Show()
    {
        this.FadeIn(200);
        pattern.SpeedTo(new Vector2(-50), 2000, Easing.OutQuint);
        text.ScaleTo(1.1f, 4000, Easing.OutQuint).Delay(2000).FadeOut(200).OnComplete(_ => results.Show());

        wedgeLeft.MoveToX(0, 2000, Easing.OutQuint);
        wedgeRight.MoveToX(0, 2000, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                Screen.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
}
