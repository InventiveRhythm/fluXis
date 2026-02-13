using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShakeEntry : PointListEntry
{
    protected override string Text => "Shake";
    protected override Colour4 Color => Theme.Shake;

    private ShakeEvent shake => Object as ShakeEvent;

    public ShakeEntry(ShakeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => shake.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{shake.Magnitude}px {shake.Duration}ms",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableLength<ShakeEvent>(Map, shake, BeatLength),
        new EditorVariableNumber<float>
        {
            Text = "Magnitude",
            TooltipText = "The magnitude (strength) of the shake effect.",
            Formatting = "0.##",
            Step = 0.1f,
            CurrentValue = shake.Magnitude,
            OnValueChanged = v =>
            {
                shake.Magnitude = v;
                Map.Update(shake);
            }
        }
    });
}
