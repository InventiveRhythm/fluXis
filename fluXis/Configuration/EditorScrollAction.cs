using System.ComponentModel;

namespace fluXis.Configuration;

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
