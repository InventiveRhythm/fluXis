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

public partial class PlayfieldMoveEntry : PointListEntry
{
    protected override string Text => "Playfield Move";
    protected override Colour4 Color => Colour4.FromHex("#01FE55");

    private PlayfieldMoveEvent move => Object as PlayfieldMoveEvent;

    public PlayfieldMoveEntry(PlayfieldMoveEvent obj)
        : base(obj)
    {
    }

    protected override ITimedObject CreateClone() => new PlayfieldMoveEvent
    {
        Time = Object.Time,
        OffsetX = move.OffsetX,
        OffsetY = move.OffsetY,
        Duration = move.Duration,
        Easing = move.Easing
    };

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{(int)move.OffsetX}x {(int)move.OffsetY}y {(int)move.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PlayfieldMoveEvent>(Map, move, BeatLength),
            new PointSettingsTextBox
            {
                Text = "Offset X",
                TooltipText = "The horizontal offset of the playfield.",
                DefaultText = move.OffsetX.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        move.OffsetX = result;
                    else
                        box.NotifyError();

                    Map.Update(move);
                }
            },
            new PointSettingsTextBox
            {
                Text = "Offset Y",
                TooltipText = "The vertical offset of the playfield.",
                DefaultText = move.OffsetY.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        move.OffsetY = result;
                    else
                        box.NotifyError();

                    Map.Update(move);
                }
            },
            new PointSettingsEasing
            {
                CurrentValue = move.Easing,
                OnValueChanged = easing =>
                {
                    move.Easing = easing;
                    Map.Update(move);
                }
            }
        });
    }
}
