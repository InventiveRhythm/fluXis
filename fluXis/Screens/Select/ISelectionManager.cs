using fluXis.Database.Maps;
using osu.Framework.Bindables;

namespace fluXis.Screens.Select;

public interface ISelectionManager
{
    RealmMap CurrentMap { get; }
    RealmMapSet CurrentMapSet { get; }

    Bindable<RealmMap> MapBindable { get; }
    Bindable<RealmMapSet> MapSetBindable { get; }

    void Select(RealmMap map);
}
