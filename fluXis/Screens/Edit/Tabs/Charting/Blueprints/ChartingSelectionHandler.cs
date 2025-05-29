using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Notes;
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

        var objs = objects.ToList();

        if (objs.Any(o => o is HitObject))
        {
            var hits = objs.OfType<HitObject>().ToArray();

            if (hits.Length > 0)
                actions.Add(new NoteRemoveAction(hits));
        }
    }
}
