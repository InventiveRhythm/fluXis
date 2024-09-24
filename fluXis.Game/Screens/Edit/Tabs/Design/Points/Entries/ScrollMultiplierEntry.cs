using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ScrollMultiplierEntry : PointListEntry
{
    protected override string Text => "Scroll Multiplier";
    protected override Colour4 Color => FluXisColors.PlayfieldRotate;

    private ScrollMultiplierEvent scroll => Object as ScrollMultiplierEvent;

    public ScrollMultiplierEntry(ScrollMultiplierEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new ScrollMultiplierEvent
    {
        Time = Object.Time,
        Duration = scroll.Duration,
        Multiplier = scroll.Multiplier,
        Easing = scroll.Easing
    };

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
            new PointSettingsEasing<ScrollMultiplierEvent>(Map, scroll)
        });
    }
}
