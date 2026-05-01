using System;
using System.Text.RegularExpressions;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Setup.Entries;

public partial class SetupNumberBox : CompositeDrawable
{
    private static readonly Regex invalid_chars_regex = new Regex(@"[^0-9.\-]", RegexOptions.Compiled); // only allow digits, '.' and '-'

    private SetupTextBox textBox;
    private readonly string title;

    public float Min { get; init; } = float.MinValue;
    public float Max { get; init; } = float.MaxValue;
    public string Default { get; init; } = string.Empty;
    public string Placeholder { get; init; } = string.Empty;
    public string TooltipText { get; init; } = string.Empty;
    public bool ReadOnly { get; init; } = false;
    public Action<float> OnChange { get; init; } = _ => { };

    public float Value
    {
        get
        {
            if (float.TryParse(textBox.Value, out float value))
                return Math.Clamp(value, Min, Max);

            if (float.TryParse(Placeholder, out value))
                return Math.Clamp(value, Min, Max);

            return 0; // worth throwing an exception?
        }
        set => textBox.Value = Math.Clamp(value, Min, Max).ToStringInvariant();
    }

    public float PlaceholderValue => float.TryParse(Placeholder, out float value) ? Math.Clamp(value, Min, Max) : 0;

    public SetupNumberBox(string title)
    {
        this.title = title;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChild = textBox = new SetupTextBox(title)
        {
            Default = Default,
            Placeholder = Placeholder,
            ReadOnly = ReadOnly,
            TooltipText = TooltipText,
            OnChange = v => OnChange.Invoke(sanitiseInput(textBox, v) ? Value : PlaceholderValue),
            OnCommit = _ => clampAndCommit(textBox)
        };
    }

    private bool sanitiseInput(SetupTextBox field, string newValue)
    {
        newValue = invalid_chars_regex.Replace(newValue, "");

        // keep only one minus sign
        if (newValue.Contains('-'))
            newValue = "-" + newValue.Replace("-", "");

        // keep only the first dot
        int firstDot = newValue.IndexOf('.');

        if (firstDot != -1)
        {
            newValue = newValue.Substring(0, firstDot + 1)
                       + newValue.Substring(firstDot + 1).Replace(".", "");
        }

        if (field.Value != newValue)
            field.Value = newValue;

        // this could return false if the field is left empty of is there is only a minus or dot
        return float.TryParse(newValue, out _);
    }

    private void clampAndCommit(SetupTextBox field)
    {
        if (!float.TryParse(field.Value, out float value))
        {
            field.Value = "";
            return;
        }

        float clamped = Math.Clamp(value, Min, Max);

        if (clamped != value)
            field.Value = clamped.ToStringInvariant();

        OnChange.Invoke(clamped);
    }
}
