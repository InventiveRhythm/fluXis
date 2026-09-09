using System.Collections.Generic;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;

namespace fluXis.Modes.Keys.Gameplay;

#nullable enable

public partial class KeysHitObjectColumn : KeysHitObjectManager
{
    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; } = null!;

    /// <summary>
    /// one-based index defining the lane of the column
    /// </summary>
    public int Index { get; set; }

    public override float HitPosition => hitPosition;
    private float hitPosition;

    public KeysHitObjectColumn(RulesetContainer ruleset, MapInfo map, MapEvents events, int idx, IEnumerable<HitObject> objs)
        : base(ruleset, map, events, objs, true)
    {
        Index = idx;
        DefaultScrollGroup = ruleset.ScrollGroups[$"${Index}"];
    }

    protected override void Update()
    {
        base.Update();

        Width = laneSwitchManager.WidthFor(Index);
        X = laneSwitchManager.PositionOf(Index);
        hitPosition = DrawHeight - laneSwitchManager.HitPosition;
    }
}
