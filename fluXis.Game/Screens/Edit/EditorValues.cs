using fluXis.Game.Map;
using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorValues
{
    public float Zoom = 2f;
    public Bindable<float> WaveformOpacity = new(.25f);
    public MapInfo MapInfo;
    public MapEvents MapEvents = new();
    public Editor Editor;
}
