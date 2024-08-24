using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Edit.Tabs.Design;

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
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var end = current.EndParameters.Strength;

        if (progress >= 1)
        {
            container.Strength = end;
            return;
        }

        var previous = events.LastOrDefault(e => e.Time < current.Time);
        var start = current.UseStartParams ? current.StartParameters.Strength : previous?.EndParameters.Strength ?? 0;

        if (progress < 0)
        {
            container.Strength = start;
            return;
        }

        container.Strength = Interpolation.ValueAt(clock.CurrentTime, start, end, current.Time, current.Time + current.Duration);
    }
}
