using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points;

public partial class ChartingSidebar : PointsSidebar
{
    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    private SelectionHandler<ITimedObject> selectionHandler => chartingContainer.BlueprintContainer.SelectionHandler;

    private SelectionInspector inspector;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        selectionHandler.SelectedObjects.CollectionChanged += (_, _) => updateSelection();
        updateSelection();
    }

    protected override PointsList CreatePointsList() => new ChartingPointsList();

    protected override Drawable CreateClosedContent() => inspector = new SelectionInspector();

    private void updateSelection()
    {
        inspector.Clear();

        switch (selectionHandler.SelectedObjects.Count)
        {
            case 0:
                inspector.AddSection("Nothing selected", "");
                break;

            case 1:
            {
                var selected = selectionHandler.SelectedObjects.Single();
                inspector.AddSection("Type", selected.GetType().ReadableName());
                inspector.AddSection("Time", TimeUtils.Format(selected.Time));

                if (selected is HitObject hit)
                {
                    inspector.AddSection("Hit Type", hit.Type switch
                    {
                        0 when !hit.LongNote => "Single",
                        0 when hit.LongNote => "Long",
                        1 => "Tick",
                        _ => "Unknown"
                    });

                    inspector.AddSection("Lane", $"{hit.Lane}");

                    switch (hit.Type)
                    {
                        case 0 when hit.LongNote:
                            inspector.AddSection("Length", $"{hit.HoldTime:#,0.##}ms");
                            inspector.AddSection("End Time", TimeUtils.Format(hit.EndTime));
                            break;
                    }
                }

                break;
            }

            default:
                inspector.AddSection("Selected", $"{selectionHandler.SelectedObjects.Count} objects");
                inspector.AddSection("Start", TimeUtils.Format(selectionHandler.SelectedObjects.Min(o => o.Time)));
                inspector.AddSection("End", TimeUtils.Format(selectionHandler.SelectedObjects.Max(o => o.Time)));

                if (selectionHandler.SelectedObjects.All(o => o is HitObject))
                {
                    var evenCount = Map.RealmMap.KeyCount % 2 == 0;
                    var laneCount = Map.RealmMap.KeyCount / 2;
                    var middleLane = laneCount + 1;

                    var leftCount = selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane <= laneCount);
                    var middleCount = selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane == middleLane);
                    var rightCount = evenCount
                        ? selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane > laneCount)
                        : selectionHandler.SelectedObjects.Count(o => ((HitObject)o).Lane > middleLane);

                    var leftPercentage = (float)leftCount / selectionHandler.SelectedObjects.Count * 100;
                    var middlePercentage = (float)middleCount / selectionHandler.SelectedObjects.Count * 100;
                    var rightPercentage = (float)rightCount / selectionHandler.SelectedObjects.Count * 100;

                    inspector.AddSection("Side Balance - Left", $"{leftCount} ({leftPercentage:0.##}%)");

                    if (!evenCount)
                        inspector.AddSection("Side Balance - Middle", $"{middleCount} ({middlePercentage:0.##}%)");

                    inspector.AddSection("Side Balance - Right", $"{rightCount} ({rightPercentage:0.##}%)");
                }

                break;
        }
    }
}
