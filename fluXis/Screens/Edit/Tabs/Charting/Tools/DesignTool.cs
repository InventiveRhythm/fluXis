using System;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Utils.Attributes;
using fluXis.Utils.Extensions;
using Midori.Utils;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class DesignTool<T> : ChartingTool, IHoldsObject
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

    public override Colour4 Color { get; }

    public T Instance { get; private set; } = new();

    public DesignTool()
    {
        Color = Theme.GetEventColor(Instance);
        if (Instance is IHasDuration d) d.Duration = 1000;
    }

    public override PlacementBlueprint CreateBlueprint()
        => Instance.CreateEditorBlueprint() ?? new ObjectPlacementBlueprint<T>(Instance);

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

    ITimedObject IHoldsObject.Object => Instance;
    void IHoldsObject.ApplyObject(ITimedObject obj) => Instance = obj.Serialize().Deserialize<T>();
    void IHoldsObject.Reset() => Instance = new T();
}

public interface IHoldsObject
{
    ITimedObject Object { get; }
    void ApplyObject(ITimedObject obj);
    void Reset();
}
