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
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ScrollMultiplierEntry : PointListEntry
{
    protected override string Text => "Scroll Multiplier";
    protected override Colour4 Color => Theme.ScrollMultiply;

    private ScrollMultiplierEvent scroll => Object as ScrollMultiplierEvent;

    public ScrollMultiplierEntry(ScrollMultiplierEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => scroll.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{scroll.Multiplier.ToStringInvariant("0.##")}x {(int)scroll.Duration}ms {scroll.Easing}",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsLength<ScrollMultiplierEvent>(Map, scroll, BeatLength),
        new PointSettingsNumber<float>()
        {
            Text = "Multiplier",
            Formatting = "0.0#",
            DefaultValue = scroll.Multiplier,
            Step = 0.1f,
            OnValueChanged = v =>
            {
                scroll.Multiplier = v;
                Map.Update(scroll);
            }
        },
        new PointSettingsEasing<ScrollMultiplierEvent>(Map, scroll),
        new PointSettingsLaneMask(Map, scroll)
    });
}
