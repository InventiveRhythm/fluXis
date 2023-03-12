using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics;

public partial class ParallaxContainer : Container
{
    public float Strength
    {
        get => strength;
        set
        {
            strength = value;
            var pos = getParallaxPosition(lastMousePos);
            this.MoveToX(pos.X, 300, Easing.OutQuint)
                .MoveToY(pos.Y, 300, Easing.OutQuint);
        }
    }

    private float strength = 10;

    private Vector2 lastMousePos;

    public ParallaxContainer()
    {
        Anchor = Origin = Anchor.Centre;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        lastMousePos = ToLocalSpace(e.ScreenSpaceMousePosition);
        var pos = getParallaxPosition(lastMousePos);

        this.MoveToX(pos.X, 300, Easing.OutQuint)
            .MoveToY(pos.Y, 300, Easing.OutQuint);

        return base.OnMouseMove(e);
    }

    private Vector2 getParallaxPosition(Vector2 mousepos)
    {
        float x = (mousepos.X - DrawSize.X / 2) / (DrawSize.X / 2);
        float y = (mousepos.Y - DrawSize.Y / 2) / (DrawSize.Y / 2);
        return new Vector2(-x * Strength, -y * Strength);
    }
}
