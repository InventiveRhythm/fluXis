using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Bindables;

namespace fluXis.Game.Screens.Edit;

public class EditorSettings
{
    public float Zoom
    {
        get => ZoomBindable.Value;
        set => ZoomBindable.Value = value;
    }

    public int SnapDivisor
    {
        get => SnapDivisorBindable.Value;
        set => SnapDivisorBindable.Value = value;
    }

    public Bindable<float> WaveformOpacity { get; } = new(.25f);
    public BindableBool FlashUnderlay { get; } = new();
    public BindableColour4 FlashUnderlayColor { get; } = new(FluXisColors.Background1);
    public Bindable<bool> ShowSamples { get; init; }
    public Bindable<bool> InvertedScroll { get; init; }

    public BindableFloat ZoomBindable { get; } = new(2f)
    {
        MinValue = 1f,
        MaxValue = 5f,
        Default = 2f,
        Precision = .1f
    };

    public BindableInt SnapDivisorBindable { get; } = new(4)
    {
        MinValue = 1,
        MaxValue = 16,
        Default = 4
    };

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
