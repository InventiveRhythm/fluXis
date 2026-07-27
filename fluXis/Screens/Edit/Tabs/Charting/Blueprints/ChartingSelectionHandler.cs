using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Generic;
using fluXis.Screens.Edit.Blueprints.Selection;
using osu.Framework.Allocation;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingSelectionHandler : SelectionHandler<ITimedObject>
{
    [Resolved]
    private EditorActionStack actions { get; set; }

    public override void Delete(IEnumerable<ITimedObject> objects)
    {
        if (objects == null) return;

        var objs = objects.ToArray();
        if (objs.Length > 0) actions.Add(new ObjectRemoveAction<ITimedObject>(objs));
    }
}
