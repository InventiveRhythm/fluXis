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
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShakeEntry : PointListEntry
{
    protected override string Text => "Shake";
    protected override Colour4 Color => FluXisColors.Shake;

    private ShakeEvent shake => Object as ShakeEvent;

    public ShakeEntry(ShakeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new ShakeEvent
    {
        Time = Object.Time,
        Magnitude = shake.Magnitude,
        Duration = shake.Duration
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{shake.Magnitude}px {shake.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<ShakeEvent>(Map, shake, BeatLength),
            new PointSettingsTextBox
            {
                Text = "Magnitude",
                TooltipText = "The magnitude (strength) of the shake effect.",
                DefaultText = shake.Magnitude.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result) && result >= 0)
                        shake.Magnitude = result;
                    else
                        box.NotifyError();

                    Map.Update(shake);
                }
            }
        });
    }
}
