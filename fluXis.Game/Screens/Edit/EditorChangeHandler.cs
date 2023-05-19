using System;

namespace fluXis.Game.Screens.Edit;

public class EditorChangeHandler
{
    public Action OnTimingPointChanged { get; set; }
    public Action OnTimingPointRemoved { get; set; }
    public Action OnTimingPointAdded { get; set; }
    public Action<int> OnKeyModeChanged { get; set; }
}
