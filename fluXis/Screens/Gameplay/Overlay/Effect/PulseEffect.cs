using System.Collections.Generic;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Gameplay.Overlay.Effect;

public partial class PulseEffect : Container
{
    public override bool RemoveCompletedTransforms => false;

    private List<PulseEvent> pulses { get; }

    public PulseEffect(List<PulseEvent> pulses)
    {
        this.pulses = pulses;

        RelativeSizeAxes = Axes.Both;
        BorderColour = Colour4.White;
        Masking = true;
        Blending = BlendingParameters.Additive;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            AlwaysPresent = true,
            Alpha = 0
        };
    }

    protected override void LoadComplete() => Rebuild();

    public void Rebuild()
    {
        ClearTransforms();
        pulses.ForEach(p => p.Apply(this));
    }
}
