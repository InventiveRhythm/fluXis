using System;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsLength<T> : PointSettingsTextBox
    where T : class, ITimedObject, IHasDuration
{
    private EditorMap map { get; }
    private T obj { get; }
    private float beatLength { get; }

    public PointSettingsLength(EditorMap map, T obj, float beatLength)
    {
        this.map = map;
        this.obj = obj;
        this.beatLength = beatLength;

        Text = "Animation Length";
        TooltipText = "The duration of the animation in beats.";
        ExtraText = "beat(s)";
        TextBoxWidth = 100;
        DefaultText = (obj.Duration / beatLength).ToStringInvariant("0.##");
        OnTextChanged = box =>
        {
            if (box.Text.TryParseFloatInvariant(out var result))
                obj.Duration = result * beatLength;
            else
                box.NotifyError();

            map.Update(obj);
        };
    }

    protected override Drawable CreateExtraButton() => new PointSettingsToCurrentButton
    {
        Action = t =>
        {
            var diff = t - obj.Time;
            diff = Math.Max(0, diff);

            TextBox.Text = (diff / beatLength).ToStringInvariant("0.##");
            obj.Duration = diff;
            map.Update(obj);
        }
    };
}
