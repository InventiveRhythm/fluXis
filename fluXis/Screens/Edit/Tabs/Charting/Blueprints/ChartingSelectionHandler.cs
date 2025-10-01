using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Notes;
using fluXis.Screens.Edit.Blueprints.Selection;
using osu.Framework.Allocation;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingSelectionHandler : SelectionHandler<HitObject>
{
    [Resolved]
    private EditorActionStack actions { get; set; }

    public override void Delete(IEnumerable<HitObject> objects)
    {
        if (objects == null) return;

        var objs = objects.ToArray();
        if (objs.Length > 0) actions.Add(new NoteRemoveAction(objs));
    }
}
