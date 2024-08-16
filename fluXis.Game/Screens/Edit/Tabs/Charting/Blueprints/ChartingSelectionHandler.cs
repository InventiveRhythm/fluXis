using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Actions.Notes;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingSelectionHandler : SelectionHandler<ITimedObject>
{
    [Resolved]
    private EditorActionStack actions { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

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

        // todo: maybe should move this one into the NoteRemoveAction?
        foreach (ITimedObject obj in objs)
        {
            switch (obj)
            {
                case FlashEvent flash:
                    map.Remove(flash);
                    break;
            }
        }
    }
}
