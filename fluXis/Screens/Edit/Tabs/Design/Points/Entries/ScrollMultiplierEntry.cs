using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ScrollMultiplierEntry : PointListEntry
{
    protected override string Text => "Scroll Multiplier";
    protected override Colour4 Color => FluXisColors.ScrollMultiply;

    private ScrollMultiplierEvent scroll => Object as ScrollMultiplierEvent;

    public ScrollMultiplierEntry(ScrollMultiplierEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => scroll.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{scroll.Multiplier.ToStringInvariant("0.##")}x {(int)scroll.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<ScrollMultiplierEvent>(Map, scroll, BeatLength),
            new PointSettingsTextBox
            {
                Text = "Multiplier",
                DefaultText = scroll.Multiplier.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        scroll.Multiplier = result;
                    else
                        box.NotifyError();

                    Map.Update(scroll);
                }
            },
            new PointSettingsEasing<ScrollMultiplierEvent>(Map, scroll),
            new PointSettingsLaneMask(Map, scroll)
        });
    }
}
