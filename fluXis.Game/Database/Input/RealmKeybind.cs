using Realms;

namespace fluXis.Game.Database.Input;

public class RealmKeybind : RealmObject
{
    public string Action { get; set; }
    public string Key { get; set; }
}
