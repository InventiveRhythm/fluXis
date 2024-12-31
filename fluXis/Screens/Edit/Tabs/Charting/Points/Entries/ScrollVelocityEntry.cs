using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Points.Entries;

public partial class ScrollVelocityEntry : PointListEntry
{
    protected override string Text => "Scroll Velocity";
    protected override Colour4 Color => FluXisColors.ScrollVelocity;

    private ScrollVelocity sv => Object as ScrollVelocity;

    public ScrollVelocityEntry(ScrollVelocity sv)
        : base(sv)
    {
    }

    public override ITimedObject CreateClone() => new ScrollVelocity
    {
        Time = Object.Time,
        Multiplier = sv.Multiplier,
        LaneMask = sv.LaneMask.ToList()
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{sv.Multiplier.ToStringInvariant("0.00")}x",
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
                Text = "Multiplier",
                TooltipText = "The speed to multiply the scroll velocity by.",
                ExtraText = "x",
                TextBoxWidth = 195,
                DefaultText = sv.Multiplier.ToStringInvariant("0.0000"),
                OnTextChanged = box =>
                {
                    if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var result))
                        sv.Multiplier = result;
                    else
                        box.NotifyError();

                    Map.Update(sv);
                }
            },
            new PointSettingsLaneMask(Map, sv)
        });
    }
}
