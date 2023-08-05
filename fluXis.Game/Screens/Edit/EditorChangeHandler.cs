using System;
using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit;

public class EditorChangeHandler
{
    // Notes
    public Action<HitObjectInfo> OnHitObjectAdded { get; set; }

    // Timing
    public Action OnTimingPointChanged { get; set; }
    public Action OnTimingPointRemoved { get; set; }
    public Action OnTimingPointAdded { get; set; }

    // Keymode
    public Action<int> OnKeyModeChanged { get; set; }

    // Snap
    public Action SnapDivisorChanged { get; set; }
}
