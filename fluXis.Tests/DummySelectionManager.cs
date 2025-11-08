using fluXis.Database.Maps;
using fluXis.Screens.Select;
using JetBrains.Annotations;
using osu.Framework.Bindables;

namespace fluXis.Tests;

public class DummySelectionManager : ISelectionManager
{
    public RealmMap CurrentMap => MapBindable.Value;
    public RealmMapSet CurrentMapSet => MapSetBindable.Value;

    public Bindable<RealmMap> MapBindable { get; } = new();
    public Bindable<RealmMapSet> MapSetBindable { get; } = new();

    public void Select([CanBeNull] RealmMap map)
    {
        MapBindable.Value = map;
        MapSetBindable.Value = map?.MapSet;
    }
}
