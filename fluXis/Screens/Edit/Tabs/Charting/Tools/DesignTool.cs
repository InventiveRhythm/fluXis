using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Utils.Attributes;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class DesignTool<T> : ChartingTool
    where T : IMapEvent, new()
{
    public override LocalisableString Name => ChartingTab.FormatTypeName<T>(title: true);
    public override LocalisableString Description => typeof(T).GetTypeDescription();

    public override Key Shortcut
    {
        get
        {
            if (key != null)
                return key.Value;

            key = Enum.Parse<Key>([Name.ToString()[0]]);
            return key.Value;
        }
    }

    private Key? key;

    public override Colour4 Color { get; } = Theme.GetEventColor(new T());

    public override PlacementBlueprint CreateBlueprint()
    {
        var obj = new T();
        return obj.CreateEditorBlueprint() ?? new ObjectPlacementBlueprint<T>(obj);
    }

    public override Drawable CreateIcon()
    {
        var icon = typeof(T).GetIcon();
        if (icon.Icon == 0x3f) return null;

        return new FluXisSpriteIcon
        {
            Icon = icon,
            Colour = Theme.GetEventColor(new T())
        };
    }
}
