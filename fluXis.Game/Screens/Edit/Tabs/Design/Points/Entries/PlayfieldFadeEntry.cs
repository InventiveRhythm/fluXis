using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PlayfieldFadeEntry : PointListEntry
{
    protected override string Text => "Playfield Fade";
    protected override Colour4 Color => Colour4.FromHex("#0180FE");

    private PlayfieldFadeEvent fade => Object as PlayfieldFadeEvent;

    public PlayfieldFadeEntry(PlayfieldFadeEvent obj)
        : base(obj)
    {
    }

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{(fade.Alpha * 100).ToStringInvariant()}% {fade.Duration.ToStringInvariant()}ms",
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
