using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class HitObjectEaseEntry : PointListEntry
{
    protected override string Text => "HitObject Ease";
    protected override Colour4 Color => FluXisColors.HitObjectEase;

    private HitObjectEaseEvent ease => Object as HitObjectEaseEvent;

    public HitObjectEaseEntry(HitObjectEaseEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new HitObjectEaseEvent
    {
        Time = Object.Time,
        Easing = ease.Easing
    };

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
