using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class BeatPulseEntry : PointListEntry
{
    protected override string Text => "Beat Pulse";
    protected override Colour4 Color => Theme.BeatPulse;

    private BeatPulseEvent pulse => Object as BeatPulseEvent;

    public BeatPulseEntry(BeatPulseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => pulse.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{pulse.Strength.ToStringInvariant("0.00")}x {(pulse.Interval * 4).ToStringInvariant("0.##")}/4",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableNumber<float>
        {
            Text = "Strength",
            TooltipText = "The strength of the pulse effect.",
            Formatting = "0.0##",
            Step = .01f,
            CurrentValue = pulse.Strength,
            OnValueChanged = v =>
            {
                pulse.Strength = v;
                Map.Update(pulse);
            }
        },
        new EditorVariableSlider<float>
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
        },
        new EditorVariableNumber<float>
        {
            Text = "Interval",
            TooltipText = "How many beats pass until the next pulse.",
            Formatting = "0.#####",
            CurrentValue = pulse.Interval,
            FetchStepValue = () => 1f / Settings.SnapDivisor,
            OnValueChanged = v =>
            {
                pulse.Interval = v;
                Map.Update(pulse);
            }
        }
    });
}
