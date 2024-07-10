using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class BeatPulseEntry : PointListEntry
{
    protected override string Text => "Beat Pulse";
    protected override Colour4 Color => FluXisColors.BeatPulse;

    private BeatPulseEvent pulse => Object as BeatPulseEvent;

    public BeatPulseEntry(BeatPulseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new BeatPulseEvent
    {
        Time = Object.Time,
        Strength = pulse.Strength,
        ZoomIn = pulse.ZoomIn
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{pulse.Strength.ToStringInvariant("0.00")}x",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsTextBox
            {
                Text = "Strength",
                TooltipText = "The strength of the pulse effect.",
                DefaultText = pulse.Strength.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        pulse.Strength = result;
                    else
                        box.NotifyError();

                    Map.Update(pulse);
                }
            },
            new PointSettingsSlider<float>
            {
                Text = "Zoom in %",
                TooltipText = "How much of the animation should be used for zooming in.",
                CurrentValue = pulse.ZoomIn,
                Min = 0,
                Max = 0.5f,
                Step = 0.05f,
                OnValueChanged = v =>
                {
                    pulse.ZoomIn = v;
                    Map.Update(pulse);
                }
            }
        });
    }
}
