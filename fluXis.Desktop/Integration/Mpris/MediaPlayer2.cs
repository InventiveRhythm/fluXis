using System.Collections.Generic;
using Midori.DBus.Attributes;

namespace fluXis.Desktop.Integration.Mpris;

[DBusInterface("org.mpris.MediaPlayer2")]
public partial class MediaPlayer2
{
    [DBusMember]
    public bool CanQuit => false;

    [DBusMember]
    public bool Fullscreen => false;

    [DBusMember]
    public bool CanSetFullscreen => false;

    [DBusMember]
    public bool CanRaise => false;

    [DBusMember]
    public bool HasTrackList => false;

    [DBusMember]
    public string Identity => "fluXis";

    [DBusMember]
    public string DesktopEntry => "";

    [DBusMember]
    public List<string> SupportedUriSchemes => [];

    [DBusMember]
    public List<string> SupportedMimeTypes => [];
}
