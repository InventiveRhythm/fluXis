using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Skinning.Default.Receptor;

public partial class DefaultReceptorUp : Container
{
    protected readonly Container Diamond;

    public DefaultReceptorUp()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            Diamond = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(24, 24),
                Masking = true,
                CornerRadius = 5,
                BorderColour = FluXisColors.Background1,
                BorderThickness = 5,
                Rotation = 45,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true,
                    Alpha = 0
                }
            }
        };
    }

    public virtual void UpdateColor(int lane, int keyCount) { }
}
