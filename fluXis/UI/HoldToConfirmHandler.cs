using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.UI;

public partial class HoldToConfirmHandler : Component
{
    public Action Action { get; set; }
    public Bindable<float> HoldTime { get; set; } = new(500);
    public bool AutoActivate { get; set; } = true;
    public bool Interpolate { get; set; }
    public Easing Easing { get; set; } = Easing.OutCubic;

    public double Progress => Interpolate ? Interpolation.ValueAt(holdTime, 0f, 1f, 0, HoldTime.Value, Easing) : holdTime / HoldTime.Value;
    public bool Finished => holdTime >= HoldTime.Value;

    private double holdTime;
    private bool isHolding;

    protected override void Update()
    {
        base.Update();

        if (!isHolding) return;

        holdTime += Time.Elapsed;

        if (!Finished || !AutoActivate) return;

        Trigger();
        isHolding = false;
    }

    public void Trigger() => Action?.Invoke();

    public void StartHold()
    {
        isHolding = true;
        FinishTransforms(true);
    }

    public void StopHold()
    {
        isHolding = false;
        this.TransformTo(nameof(holdTime), 0d, 200, Easing.OutCubic);
    }
}
