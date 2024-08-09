using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Waveform;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class TimingPointEntry : PointListEntry
{
    protected override string Text => "Timing Point";
    protected override Colour4 Color => FluXisColors.TimingPoint;

    private TimingPoint timing => Object as TimingPoint;

    public TimingPointEntry(TimingPoint timing)
        : base(timing)
    {
    }

    public override ITimedObject CreateClone() => new TimingPoint
    {
        Time = Object.Time,
        BPM = timing.BPM,
        Signature = timing.Signature,
        HideLines = timing.HideLines
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{timing.BPM.ToStringInvariant("0.0")}bpm {timing.Signature}/4",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new WaveformDisplay(timing),
            new PointSettingsTextBox
            {
                Text = "BPM",
                TooltipText = "The beats per minute of the timing point.",
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
            }
        });
    }
}
