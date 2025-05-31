using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class BigBlackBox : GameplayHUDComponent
{
    [BindableSetting("Spin Text", "", "spin")]
    public BindableBool SpinText { get; } = new();

    [BindableSetting("Alpha", "The opacity of the background.", "bg-alpha")]
    public BindableFloat BackgroundAlpha { get; } = new(.5f) { MinValue = 0, MaxValue = 1 };

    private Box background;
    private FluXisSpriteText text;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(256);
        CornerRadius = 16;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
            },
            text = new FluXisSpriteText
            {
                Text = "Big Black Box",
                FontSize = 32,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        BackgroundAlpha.BindValueChanged(v => background.Alpha = v.NewValue, true);
        SpinText.BindValueChanged(v =>
        {
            if (v.NewValue)
                text.Spin(1000, RotationDirection.Clockwise);
            else
                text.FinishTransforms();
        }, true);
    }
}
