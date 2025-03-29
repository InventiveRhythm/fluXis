﻿using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Blueprints.Selection;
using osu.Framework.Allocation;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class ChartingSelectionBlueprint : SelectionBlueprint<ITimedObject>
{
    [Resolved]
    protected EditorSnapProvider Snaps { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected ChartingContainer ChartingContainer { get; private set; }

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object.Time;

    protected ChartingSelectionBlueprint(ITimedObject info)
        : base(info)
    {
    }
}
