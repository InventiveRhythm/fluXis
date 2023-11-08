using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorValues
{
    public float Zoom
    {
        get => ZoomBindable.Value;
        set => ZoomBindable.Value = value;
    }

    public int SnapDivisor { get; set; } = 4;
    public Bindable<float> WaveformOpacity { get; } = new(.25f);
    public BindableBool FlashUnderlay { get; } = new();
    public BindableColour4 FlashUnderlayColor { get; } = new(FluXisColors.Background1);
    public EditorMapInfo MapInfo { get; init; }
    public EditorMapEvents MapEvents { get; init; } = new();
    public Editor Editor { get; init; }
    public EditorActionStack ActionStack { get; init; }

    public BindableFloat ZoomBindable { get; } = new(2f)
    {
        MinValue = 1f,
        MaxValue = 5f,
        Default = 2f,
        Precision = .1f
    };

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
