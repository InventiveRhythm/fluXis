using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Settings;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.List.Entries;

public partial class ScrollVelocityEntry : PointListEntry
{
    protected override string Text => "Scroll Velocity";
    protected override Colour4 Color => Colour4.FromHex("#00D4FF");

    private ScrollVelocity sv => Object as ScrollVelocity;

    public ScrollVelocityEntry(ScrollVelocity sv)
        : base(sv)
    {
    }

    protected override string CreateValueText() => $"{sv.Multiplier.ToStringInvariant("0.00")}x";

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsTextBox
            {
                Text = "Multiplier",
                ExtraText = "x",
                TextBoxWidth = 195,
                DefaultText = sv.Multiplier.ToStringInvariant("0.00"),
                OnTextChanged = box =>
                {
                    if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var result))
                        sv.Multiplier = result;
                    else
                        box.NotifyError();

                    Values.MapInfo.Update(sv);
                }
            }
        });
    }
}
