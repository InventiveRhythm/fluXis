using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Scrolling;

public partial class HitObjectEaseEntry : PointListEntry
{
    protected override string Text => "HitObject Ease";
    protected override Colour4 Color => Theme.HitObjectEase;

    private HitObjectEaseEvent ease => Object as HitObjectEaseEvent;

    public HitObjectEaseEntry(HitObjectEaseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => ease.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{ease.Easing}",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsEasing<HitObjectEaseEvent>(Map, ease)
    });
}
