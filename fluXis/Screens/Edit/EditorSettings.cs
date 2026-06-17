using fluXis.Screens.Edit.Input;
using osu.Framework.Bindables;

namespace fluXis.Screens.Edit;

public class EditorSettings
{
    private readonly EditorKeybindingContainer bindings;
    public EditorKeymap Keymap => bindings.Keymap;

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
    public BindableBool ForceAspectRatio { get; } = new();

    public BindableDouble ZoomBindable { get; } = new(2f)
    {
        MinValue = .5f,
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

    public EditorSettings(EditorKeybindingContainer bindings)
    {
        this.bindings = bindings;
    }

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
