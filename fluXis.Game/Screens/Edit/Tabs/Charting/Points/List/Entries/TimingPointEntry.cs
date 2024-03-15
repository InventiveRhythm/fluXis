using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings.Waveform;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.List.Entries;

public partial class TimingPointEntry : PointListEntry
{
    protected override string Text => "Timing Point";
    protected override Colour4 Color => Colour4.FromHex("#00FF80");

    private TimingPoint timing => Object as TimingPoint;

    public TimingPointEntry(TimingPoint timing)
        : base(timing)
    {
    }

    protected override string CreateValueText() => $"{timing.BPM.ToStringInvariant("0.0")}bpm {timing.Signature}/4";

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new WaveformDisplay(timing),
            new PointSettingsTextBox
            {
                Text = "BPM",
                DefaultText = timing.BPM.ToStringInvariant("0.00"),
                OnTextChanged = box =>
                {
                    if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var result))
                        timing.BPM = result;
                    else
                        box.NotifyError();

                    Map.Update(timing);
                }
            },
            new PointSettingsTextBox
            {
                Text = "Time Signature",
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
            }
        });
    }
}
