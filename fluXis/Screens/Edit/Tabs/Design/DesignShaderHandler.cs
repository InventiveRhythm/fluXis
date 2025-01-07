using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Shaders;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Screens.Edit.Tabs.Design;

public partial class DesignShaderHandler : CompositeComponent
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public ShaderStackContainer ShaderStack { get; set; }

    protected override void Update()
    {
        base.Update();

        if (ShaderStack is null)
            return;

        var groups = map.MapEvents.ShaderEvents.GroupBy(x => x.Type);

        foreach (var group in groups)
            handleGroup(group.Key, group);
    }

    private void handleGroup(ShaderType type, IEnumerable<ShaderEvent> events)
    {
        var container = ShaderStack.GetShader(type);

        if (container == null)
            return;

        var current = events.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            container.Strength = 0;
            container.Strength2 = 0;
            container.Strength3 = 0;
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var endStrength = current.EndParameters.Strength;
        var endStrength2 = current.EndParameters.Strength2;
        var endStrength3 = current.EndParameters.Strength3;

        if (progress >= 1)
        {
            container.Strength = endStrength;
            container.Strength2 = endStrength2;
            container.Strength3 = endStrength3;
            return;
        }

        var previous = events.LastOrDefault(e => e.Time < current.Time);
        var startStrength = current.UseStartParams ? current.StartParameters.Strength : previous?.EndParameters.Strength ?? 0;
        var startStrength2 = current.UseStartParams ? current.StartParameters.Strength2 : previous?.EndParameters.Strength2 ?? 0;
        var startStrength3 = current.UseStartParams ? current.StartParameters.Strength3 : previous?.EndParameters.Strength3 ?? 0;

        if (progress < 0)
        {
            container.Strength = startStrength;
            container.Strength2 = startStrength2;
            container.Strength3 = startStrength3;
            return;
        }

        container.Strength = Interpolation.ValueAt(clock.CurrentTime, startStrength, endStrength, current.Time, current.Time + current.Duration);
        container.Strength2 = Interpolation.ValueAt(clock.CurrentTime, startStrength2, endStrength2, current.Time, current.Time + current.Duration);
        container.Strength3 = Interpolation.ValueAt(clock.CurrentTime, startStrength3, endStrength3, current.Time, current.Time + current.Duration);
    }
}
