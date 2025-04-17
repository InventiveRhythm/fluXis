using System;
using fluXis.Map.Structures.Bases;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public abstract partial class PointSettingsBeats<T> : PointSettingsTextBox
    where T : class, ITimedObject
{
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
        DefaultText = (Value / beatLength).ToStringInvariant("0.##");
        OnTextChanged = box =>
        {
            if (box.Text.TryParseFloatInvariant(out var result))
                Value = result * beatLength;
            else
                box.NotifyError();

            map.Update(obj);
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
