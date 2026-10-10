using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Overlay.Result;

public partial class ResultShearContainer : Container
{
    protected override Container<Drawable> Content => container;
    private readonly Container container;

    public ResultShearContainer()
    {
        Masking = true;
        InternalChild = container = new Container
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        container.Shear = -Shear;
        container.Anchor = Anchor.Opposite();
        container.Origin = Origin.Opposite();
    }

    protected override void Update()
    {
        base.Update();

        container.X = -X;
    }
}
