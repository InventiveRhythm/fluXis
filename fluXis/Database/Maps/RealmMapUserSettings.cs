using Realms;

namespace fluXis.Database.Maps;

[MapTo("MapUserSettings")]
public class RealmMapUserSettings : EmbeddedObject
{
    public float Offset { get; set; }
    public float? ScrollSpeed { get; set; }

    public bool DisableColors { get; set; }
    public bool DisableHitSounds { get; set; }
}
