using Realms;

namespace fluXis.Game.Database;

public class ImporterInfo : RealmObject
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Color { get; set; }
}
