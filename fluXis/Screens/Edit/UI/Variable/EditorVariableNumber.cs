using System;
using System.Globalization;
using System.Numerics;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK.Input;

namespace fluXis.Screens.Edit.UI.Variable;

public partial class EditorVariableNumber<T> : EditorVariableTextBox, IHasCursorType
    where T : struct, INumber<T>, IMinMaxValue<T>
{
    public CursorType Cursor => CursorType.SizeHorizontal;

    // hide
    public new Action<FluXisTextBox> OnCommit { get; private set; }

    public override Type CommitEvalType => typeof(T);

    public new T CurrentValue
    {
        set
        {
            UpdateValue(value, false);
            UpdateText();
        }
    }

    public T Step { get; init; } = T.One;
    public Func<T?> FetchStepValue { get; init; }

    private T stepValue => FetchStepValue?.Invoke() ?? Step;

    public string Formatting { get; set; } = "0";
    public new Action<T> OnValueChanged { get; init; }

    public T? Min { get; init; }
    public T? Max { get; init; }

    private readonly Bindable<T> bind = new();

    private Sample changeSample;
    private double lastSampleTime;

    public EditorVariableNumber()
    {
        base.OnValueChanged += t =>
        {
            if (t.Text.TryEvaluateTo<T>(out var val))
                UpdateValue(val, true);
            else
                t.NotifyError();
        };

        base.OnCommit += _ => UpdateText();
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        changeSample = samples.Get("UI/slider-tick");
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        return true;
    }

    private float accumulated;

    protected override void OnDrag(DragEvent e)
    {
        accumulated += e.Delta.X;

        T change = T.Zero;
        T step = stepValue;

        while (accumulated >= 10)
        {
            change += step;
            accumulated -= 10;
        }

        while (accumulated <= -10)
        {
            change -= step;
            accumulated += 10;
        }

        if (change == T.Zero)
            return;

        if (Math.Abs(Time.Current - lastSampleTime) > 50)
        {
            changeSample?.Play();
            lastSampleTime = Time.Current;
        }

        UpdateValue(bind.Value += change, true);
        UpdateText();
    }

    protected void UpdateValue(T newVal, bool notify)
    {
        if (Min != null)
            newVal = T.Max(Min!.Value, newVal);
        if (Max != null)
            newVal = T.Min(Max!.Value, newVal);

        bind.Value = newVal;

        if (notify)
            OnValueChanged?.Invoke(bind.Value);
    }

    protected void UpdateText()
    {
        var text = bind.Value.ToString(Formatting, CultureInfo.InvariantCulture);
        Schedule(() => TextBox.Text = text);
    }

    protected override void EvalCommit(FluXisTextBox box)
    {
        if (!box.IsAlive || box is null) return;

        if (box.Text.TryEvaluateTo<T>(out var result))
        {
            UpdateValue(result, true);
        }
        else
            box.NotifyError();

        UpdateText();
    }
}
