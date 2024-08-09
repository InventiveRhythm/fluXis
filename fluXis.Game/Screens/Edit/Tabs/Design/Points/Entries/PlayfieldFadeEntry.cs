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

public partial class PlayfieldFadeEntry : PointListEntry
{
    protected override string Text => "Playfield Fade";
    protected override Colour4 Color => FluXisColors.PlayfieldFade;

    private PlayfieldFadeEvent fade => Object as PlayfieldFadeEvent;

    public PlayfieldFadeEntry(PlayfieldFadeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new PlayfieldFadeEvent
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
            new PointSettingsLength<PlayfieldFadeEvent>(Map, fade, BeatLength),
            new PointSettingsSlider<float>
            {
                Text = "Alpha",
                TooltipText = "The opacity of the playfield.",
                CurrentValue = fade.Alpha,
                Min = 0,
                Max = 1,
                OnValueChanged = v =>
                {
                    fade.Alpha = v;
                    Map.Update(fade);
                }
            },
            new PointSettingsEasing<PlayfieldFadeEvent>(Map, fade)
        });
    }
}
