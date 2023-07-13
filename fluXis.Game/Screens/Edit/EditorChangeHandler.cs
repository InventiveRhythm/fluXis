using System;

namespace fluXis.Game.Screens.Edit;

public class EditorChangeHandler
{
    // Timing
    public Action OnTimingPointChanged { get; set; }
    public Action OnTimingPointRemoved { get; set; }
    public Action OnTimingPointAdded { get; set; }

    // Keymode
    public Action<int> OnKeyModeChanged { get; set; }

    // Snap
    public Action SnapDivisorChanged { get; set; }
}
