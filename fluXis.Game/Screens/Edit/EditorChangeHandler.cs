using System;

namespace fluXis.Game.Screens.Edit;

public class EditorChangeHandler
{
    public Action<int> OnKeyModeChanged { get; set; }

    // Snap
    public Action SnapDivisorChanged { get; set; }
}
