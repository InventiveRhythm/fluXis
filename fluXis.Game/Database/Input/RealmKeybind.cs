using System;
using Realms;

namespace fluXis.Game.Database.Input;

public class RealmKeybind : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public string Action { get; set; }

    public string Key { get; set; }

    public RealmKeybind()
    {
        ID = Guid.NewGuid();
    }
}
