using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Screens.Edit.UI.Variable.Timing;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class TimingPointEntry : PointListEntry
{
    protected override string Text => "Timing Point";
    protected override Colour4 Color => Theme.TimingPoint;

    private TimingPoint timing => Object as TimingPoint;

    [CanBeNull]
    private EditorVariableTime timeBox;

    [CanBeNull]
    private EditorVariableTextBox bpmBox;

    public TimingPointEntry(TimingPoint timing)
        : base(timing)
    {
    }

    public override ITimedObject CreateClone() => timing.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{timing.BPM.ToStringInvariant("0.0")}bpm {timing.Signature}/4",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override void OnValueUpdate()
    {
        if (timeBox is not null && !timeBox.TextBox.HasFocus)
            timeBox.TextBox.Text = timing.Time.ToStringInvariant("0");
        if (bpmBox is not null && !bpmBox.TextBox.HasFocus)
            bpmBox.TextBox.Text = timing.BPM.ToStringInvariant("0.##");
    }

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Take(1).Concat(new Drawable[]
    {
        timeBox = new EditorVariableTime(Map, Object),
        new EditorVariableWaveform(timing),
        new EditorVariableIncrements(Map, timing),
        bpmBox = new EditorVariableTextBox
        {
            Text = "BPM",
            TooltipText = "The beats per minute of the timing point.",
            CurrentValue = timing.BPM.ToStringInvariant("0.##"),
            OnValueChanged = box =>
            {
                if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var result) && result > 0)
                    timing.BPM = result;
                else
                    box.NotifyError();

                Map.Update(timing);
            }
        },
        new EditorVariableTextBox
        {
            Text = "Time Signature",
            TooltipText = "The time signature of the timing point.",
            ExtraText = "/ 4",
            TextBoxWidth = 50,
            CurrentValue = timing.Signature.ToString(),
            OnValueChanged = box =>
            {
                if (int.TryParse(box.Text, CultureInfo.InvariantCulture, out var result))
                    timing.Signature = result;
                else
                    box.NotifyError();

                Map.Update(timing);
            }
        },
        new EditorVariableToggle
        {
            Text = "Hide Lines",
            TooltipText = "Hides the lines that appear every 4 beats during gameplay.",
            Bindable = new Bindable<bool>(timing.HideLines),
            OnValueChanged = enabled =>
            {
                timing.HideLines = enabled;
                Map.Update(timing);
            }
        }
    });
}
