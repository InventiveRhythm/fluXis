using System.Collections.Generic;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay;

public partial class CameraContainer : Container
{
    protected override bool RequiresChildrenUpdate => true;

    private Drawable proxy;
    private readonly List<ICameraEvent> events;

    public CameraContainer(List<ICameraEvent> events)
    {
        this.events = events;

        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Refresh(events);
    }

    protected override void Update()
    {
        base.Update();

        Position = -proxy.Position;
        Scale = proxy.Scale;
        Rotation = proxy.Rotation;
    }

    public Drawable CreateProxyDrawable() => proxy = new CameraProxy();

    public void Refresh(List<ICameraEvent> ev)
    {
        proxy.ClearTransforms(true);

        using (proxy.BeginAbsoluteSequence(-10000))
            proxy.MoveToX(0).MoveToY(0).ScaleTo(1f).RotateTo(0);

        ev.ForEach(x => x.Apply(proxy));
    }

    private partial class CameraProxy : Drawable
    {
        public override bool RemoveCompletedTransforms => false;
    }
}
