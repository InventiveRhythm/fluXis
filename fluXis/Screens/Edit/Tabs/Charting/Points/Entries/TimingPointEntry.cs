using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Waveform;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class TimingPointEntry : PointListEntry
{
    protected override string Text => "Timing Point";
    protected override Colour4 Color => FluXisColors.TimingPoint;

    private TimingPoint timing => Object as TimingPoint;

    [CanBeNull]
    private PointSettingsTime timeBox;

    [CanBeNull]
    private PointSettingsTextBox bpmBox;

    public TimingPointEntry(TimingPoint timing)
        : base(timing)
    {
    }

    public override ITimedObject CreateClone() => timing.JsonCopy();

    protected override Drawable[] CreateValueContent() => new Drawable[]
    {
        new FluXisSpriteText
        {
            Text = $"{timing.BPM.ToStringInvariant("0.0")}bpm {timing.Signature}/4",
            Colour = Color
        }
    };

    protected override void OnValueUpdate()
    {
        if (timeBox is not null)
            timeBox.TextBox.Text = timing.Time.ToStringInvariant("0");
        if (bpmBox is not null)
            bpmBox.TextBox.Text = timing.BPM.ToStringInvariant("0.##");
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Take(1).Concat(new Drawable[]
        {
            timeBox = new PointSettingsTime(Map, Object),
            new WaveformDisplay(timing),
            new PointSettingsIncrements(Map, timing),
            bpmBox = new PointSettingsTextBox
            {
                Text = "BPM",
                TooltipText = "The beats per minute of the timing point.",
                DefaultText = timing.BPM.ToStringInvariant("0.##"),
                OnTextChanged = box =>
                {
                    if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var result) && result > 0)
                        timing.BPM = result;
                    else
                        box.NotifyError();

                    Map.Update(timing);
                }
            },
            new PointSettingsTextBox
            {
                Text = "Time Signature",
                TooltipText = "The time signature of the timing point.",
                ExtraText = "/ 4",
                TextBoxWidth = 50,
                DefaultText = timing.Signature.ToString(),
                OnTextChanged = box =>
                {
                    if (int.TryParse(box.Text, CultureInfo.InvariantCulture, out var result))
                        timing.Signature = result;
                    else
                        box.NotifyError();

                    Map.Update(timing);
                }
            },
            new PointSettingsToggle
            {
                Text = "Hide Lines",
                TooltipText = "Hides the lines that appear every 4 beats during gameplay.",
                Bindable = new Bindable<bool>(timing.HideLines),
                OnStateChanged = enabled =>
                {
                    timing.HideLines = enabled;
                    Map.Update(timing);
                }
            }
        });
    }
}
