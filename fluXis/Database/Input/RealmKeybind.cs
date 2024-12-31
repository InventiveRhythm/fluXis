using System;
using Realms;

namespace fluXis.Database.Input;

public class RealmKeybind : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; } = Guid.NewGuid();

    public string Action { get; set; }

    public string Key { get; set; }
}
