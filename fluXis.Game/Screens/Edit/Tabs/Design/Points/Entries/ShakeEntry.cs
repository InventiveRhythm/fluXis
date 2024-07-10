using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

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
