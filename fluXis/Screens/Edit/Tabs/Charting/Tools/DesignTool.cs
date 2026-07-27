using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class DesignTool<T> : ChartingTool
    where T : IMapEvent, new()
{
    public override string Name => ChartingTab.FormatTypeName<T>(title: true);
    public override string Description => "yea";

    public override Key Shortcut
    {
        get
        {
            if (key != null)
                return key.Value;

            key = Enum.Parse<Key>([Name[0]]);
            return key.Value;
        }
    }

    private Key? key;

    public override Colour4 Color { get; } = Theme.GetEventColor(new T());

    public override PlacementBlueprint CreateBlueprint() => new ObjectPlacementBlueprint<T>(new T());
}
