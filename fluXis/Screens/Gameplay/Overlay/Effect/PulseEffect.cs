using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Gameplay.Overlay.Effect;

public partial class PulseEffect : Container
{
    public override bool RemoveCompletedTransforms => false;

    public List<PulseEvent> Pulses { get; set; }

    public PulseEffect(List<PulseEvent> pulses)
    {
        Pulses = pulses;

        RelativeSizeAxes = Axes.Both;
        BorderColour = Colour4.White;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            AlwaysPresent = true,
            Alpha = 0
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        ClearTransforms();

        // Explicitly reset state, if we don't do this, seeking backwards PulseEffect gets stuck with
        // whatever Transforms it had
        Alpha = 1;
        Scale = Vector2.One;
        BorderThickness = 0;

        Pulses.ForEach(p => p.Apply(this));
    }
}
