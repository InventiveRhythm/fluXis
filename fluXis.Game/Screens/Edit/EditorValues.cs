using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorValues
{
    public float Zoom = 2f;
    public Bindable<float> WaveformOpacity = new(1f);
}
