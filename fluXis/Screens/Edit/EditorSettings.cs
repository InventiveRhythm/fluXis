using fluXis.Configuration;
using fluXis.Screens.Edit.Input;
using osu.Framework.Bindables;

namespace fluXis.Screens.Edit;

public class EditorSettings
{
    private readonly EditorKeybindingContainer bindings;
    public EditorKeymap Keymap => bindings.Keymap;

    public float ObjectZoom => (float)(Zoom / 2f);

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
    public Bindable<bool> ApplyZoomToPreview { get; }

    public BindableDouble ZoomBindable { get; } = new(2f)
    {
        MinValue = 1f,
        MaxValue = 2f,
        Default = 2f,
        Precision = .1f
    };

    public BindableInt SnapDivisorBindable { get; } = new(4)
    {
        MinValue = 1,
        MaxValue = 32,
        Default = 4
    };

    public EditorSettings(FluXisConfig config, EditorKeybindingContainer bindings)
    {
        ApplyZoomToPreview = config.GetBindable<bool>(FluXisSetting.EditorZoomPreview);
        this.bindings = bindings;
    }

    public override string ToString()
    {
        return $"Zoom: {Zoom}, SnapDivisor: {SnapDivisor}, WaveformOpacity: {WaveformOpacity}";
    }
}
