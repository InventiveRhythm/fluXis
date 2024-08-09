using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class HitObjectFadeEntry : PointListEntry
{
    protected override string Text => "HitObject Fade";
    protected override Colour4 Color => FluXisColors.HitObjectFade;

    private HitObjectFadeEvent fade => Object as HitObjectFadeEvent;

    public HitObjectFadeEntry(HitObjectFadeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new HitObjectFadeEvent
    {
        Time = Object.Time,
        Duration = fade.Duration,
        Alpha = fade.Alpha
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{(fade.Alpha * 100).ToStringInvariant()}% {(int)fade.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<HitObjectFadeEvent>(Map, fade, BeatLength),
            new PointSettingsSlider<float>
            {
                Text = "Alpha",
                TooltipText = "The opacity of the hitobjects.",
                CurrentValue = fade.Alpha,
                Min = 0,
                Max = 1,
                OnValueChanged = v =>
                {
                    fade.Alpha = v;
                    Map.Update(fade);
                }
            }
        });
    }
}
