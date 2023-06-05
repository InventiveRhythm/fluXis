using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Graphics;

public partial class LoadingIcon : SpriteIcon
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(100);
        Icon = FontAwesome.Solid.CircleNotch;
    }

    protected override void LoadComplete()
    {
        this.Spin(1000, RotationDirection.Clockwise);
        base.LoadComplete();
    }
}
