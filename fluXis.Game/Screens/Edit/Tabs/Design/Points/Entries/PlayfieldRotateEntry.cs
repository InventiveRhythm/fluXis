using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PlayfieldRotateEntry : PointListEntry
{
    protected override string Text => "Playfield Rotate";
    protected override Colour4 Color => Colour4.FromHex("#01FE55");

    private PlayfieldRotateEvent rotate => Object as PlayfieldRotateEvent;

    public PlayfieldRotateEntry(PlayfieldRotateEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new PlayfieldRotateEvent
    {
        Time = Object.Time,
        Roll = rotate.Roll,
        Duration = rotate.Duration,
        Easing = rotate.Easing
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{(int)rotate.Roll}deg {(int)rotate.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PlayfieldRotateEvent>(Map, rotate, BeatLength),
            new PointSettingsTextBox
            {
                Text = "Rotation",
                TooltipText = "The rotation of the playfield.",
                DefaultText = rotate.Roll.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        rotate.Roll = result;
                    else
                        box.NotifyError();

                    Map.Update(rotate);
                }
            },
            new PointSettingsEasing
            {
                CurrentValue = rotate.Easing,
                OnValueChanged = easing =>
                {
                    rotate.Easing = easing;
                    Map.Update(rotate);
                }
            }
        });
    }
}
