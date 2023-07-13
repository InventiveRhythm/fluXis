using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Compose;
using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorValues
{
    public float Zoom = 2f;
    public int SnapDivisor = 4;
    public EditorTool Tool = EditorTool.Select;
    public Bindable<float> WaveformOpacity = new(.25f);
    public MapInfo MapInfo;
    public MapEvents MapEvents = new();
    public Editor Editor;
}
