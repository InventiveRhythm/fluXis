using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Scrolling;

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
        new EditorVariableLength<ScrollMultiplierEvent>(Map, scroll, BeatLength),
        new EditorVariableNumber<float>()
        {
            Text = "Multiplier",
            Formatting = "0.0#",
            CurrentValue = scroll.Multiplier,
            Step = 0.1f,
            OnValueChanged = v =>
            {
                scroll.Multiplier = v;
                Map.Update(scroll);
            }
        },
        new EditorVariableEasing<ScrollMultiplierEvent>(Map, scroll),
        new EditorVariableLaneMask(Map, scroll)
    });
}
