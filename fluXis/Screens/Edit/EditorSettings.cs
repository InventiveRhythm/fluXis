using osu.Framework.Bindables;

namespace fluXis.Screens.Edit;

public class EditorSettings
{
    public double Zoom
    {
        get => ZoomBindable.Value;
        set => ZoomBindable.Value = value;
    }

    public int SnapDivisor
    {
        get => SnapDivisorBindable.Value;
        set => SnapDivisorBindable.Value = value;
    }

    public Bindable<float> WaveformOpacity { get; } = new(.2f);
    public Bindable<bool> ShowSamples { get; init; }
    public Bindable<bool> InvertedScroll { get; } = new();
    public Bindable<bool> LaneSwitchInteraction { get; } = new();
    public BindableBool ForceAspectRatio { get; } = new();

    public BindableDouble ZoomBindable { get; } = new(2f)
    {
        MinValue = 1f,
        MaxValue = 5f,
        Default = 2f,
        Precision = .1f
    };

    public BindableInt SnapDivisorBindable { get; } = new(4)
    {
        MinValue = 1,
        MaxValue = 32,
        Default = 4
    };

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
