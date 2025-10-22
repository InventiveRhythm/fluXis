using System;
using fluXis.Map.Structures.Bases;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public abstract partial class PointSettingsBeats<T> : PointSettingsNumber<double>
    where T : class, ITimedObject
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private EditorSettings settings { get; set; }

    protected T Object { get; }
    private EditorMap map { get; }
    private float beatLength { get; }

    protected abstract double Value { get; set; }

    protected PointSettingsBeats(EditorMap map, T obj, float beatLength)
    {
        this.map = map;
        Object = obj;
        this.beatLength = beatLength;

        ExtraText = "beat(s)";
        TextBoxWidth = 100;
        Formatting = "0.##";
        DefaultValue = Value / beatLength;
        Min = 0;
        OnValueChanged = v =>
        {
            Value = v * beatLength;
            map.Update(obj);
        };

        FetchStepValue = () =>
        {
            if (settings is null)
                return 1;

            return 1f / settings.SnapDivisor;
        };
    }

    protected override Drawable CreateExtraButton() => new PointSettingsToCurrentButton
    {
        Action = t =>
        {
            var diff = t - Object.Time;
            diff = Math.Max(0, diff);

            TextBox.Text = (diff / beatLength).ToStringInvariant("0.##");
            Value = diff;
            map.Update(Object);
        }
    };
}
