using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class HitObjectEaseEntry : PointListEntry
{
    protected override string Text => "HitObject Ease";
    protected override Colour4 Color => FluXisColors.HitObjectEase;

    private HitObjectEaseEvent ease => Object as HitObjectEaseEvent;

    public HitObjectEaseEntry(HitObjectEaseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => ease.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{ease.Easing}",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsEasing<HitObjectEaseEvent>(Map, ease)
        });
    }
}
