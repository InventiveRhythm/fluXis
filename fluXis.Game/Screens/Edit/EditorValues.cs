using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Compose;
using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorValues
{
    public float Zoom { get; set; } = 2f;
    public int SnapDivisor { get; set; } = 4;
    public EditorTool Tool { get; set; } = EditorTool.Select;
    public Bindable<float> WaveformOpacity { get; } = new(.25f);
    public EditorMapInfo MapInfo { get; init; }
    public MapEvents MapEvents { get; init; } = new();
    public Editor Editor { get; init; }

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
