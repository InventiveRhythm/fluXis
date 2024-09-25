using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class TimeOffsetEntry : PointListEntry
{
    protected override string Text => "Time Offset";
    protected override Colour4 Color => FluXisColors.TimeOffset;

    private TimeOffsetEvent offset => Object as TimeOffsetEvent;

    public TimeOffsetEntry(TimeOffsetEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new TimeOffsetEvent
    {
        Time = Object.Time,
        Duration = offset.Duration,
        Offset = offset.Offset,
        Easing = offset.Easing
    };

    protected override Drawable[] CreateValueContent() => new Drawable[]
    {
        new FluXisSpriteText
        {
            Text = $"{offset.Offset.ToStringInvariant("0.##")}ms {(int)offset.Duration}ms",
            Colour = Color
        }
    };

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsLength<TimeOffsetEvent>(Map, offset, BeatLength),
        new PointSettingsTextBox
        {
            Text = "Offset",
            TooltipText = "The visual offset in milliseconds.",
            DefaultText = offset.Offset.ToStringInvariant(),
            OnTextChanged = box =>
            {
                if (box.Text.TryParseIntInvariant(out var result))
                    offset.Offset = result;
                else
                    box.NotifyError();

                Map.Update(offset);
            }
        },
        new PointSettingsEasing<TimeOffsetEvent>(Map, offset)
    });
}
