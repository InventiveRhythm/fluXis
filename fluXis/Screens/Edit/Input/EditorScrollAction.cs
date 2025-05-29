using System.ComponentModel;

namespace fluXis.Screens.Edit.Input;

public enum EditorScrollAction
{
    [Description("Seek Timeline")]
    Seek,

    [Description("Cycle Snaps")]
    Snap,

    [Description("Zoom Playfield")]
    Zoom,

    [Description("Change Rate")]
    Rate
}
