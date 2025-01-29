using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PulseEntry : PointListEntry
{
    protected override string Text => "Pulse";
    protected override Colour4 Color => FluXisColors.Pulse;

    private PulseEvent pulse => Object as PulseEvent;

    public PulseEntry(PulseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => pulse.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{pulse.Width.ToStringInvariant("0")}px {(int)pulse.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PulseEvent>(Map, pulse, BeatLength),
            new PointSettingsSlider<float>
            {
                Text = "Width",
                TooltipText = "How many pixels in width the pulse should have.",
                CurrentValue = pulse.Width,
                Min = 1,
                Max = 128,
                Step = 1,
                OnValueChanged = v =>
                {
                    pulse.Width = v;
                    Map.Update(pulse);
                }
            },
            new PointSettingsSlider<float>
            {
                Text = "In %",
                TooltipText = "How much of the animation should be used for going to the width.",
                CurrentValue = pulse.InPercent,
                Min = 0,
                Max = 0.5f,
                Step = 0.05f,
                OnValueChanged = v =>
                {
                    pulse.InPercent = v;
                    Map.Update(pulse);
                }
            },
            new PointSettingsEasing<PulseEvent>(Map, pulse)
        });
    }
}
