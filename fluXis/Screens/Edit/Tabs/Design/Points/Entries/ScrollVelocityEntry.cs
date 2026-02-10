using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ScrollVelocityEntry : PointListEntry
{
    protected override string Text => "Scroll Velocity";
    protected override Colour4 Color => Theme.ScrollVelocity;

    private ScrollVelocity sv => Object as ScrollVelocity;

    public ScrollVelocityEntry(ScrollVelocity sv)
        : base(sv)
    {
    }

    public override ITimedObject CreateClone() => sv.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{sv.Multiplier.ToStringInvariant("0.00")}x",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableNumber<double>
        {
            Text = "Multiplier",
            TooltipText = "The speed to multiply the scroll velocity by.",
            ExtraText = "x",
            TextBoxWidth = 195,
            Formatting = "0.0000",
            CurrentValue = sv.Multiplier,
            OnValueChanged = v =>
            {
                sv.Multiplier = v;
                Map.Update(sv);
            }
        },
        new EditorVariableLaneMask(Map, sv)
    });
}
