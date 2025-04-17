using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PlayfieldMoveEntry : PointListEntry
{
    protected override string Text => "Playfield Move";
    protected override Colour4 Color => FluXisColors.PlayfieldMove;

    private PlayfieldMoveEvent move => Object as PlayfieldMoveEvent;

    public PlayfieldMoveEntry(PlayfieldMoveEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => move.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{(int)move.OffsetX}x {(int)move.OffsetY}y {(int)move.OffsetZ}z {(int)move.Duration}ms P{move.PlayfieldIndex}S{move.PlayfieldSubIndex}",
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
            new PointSettingsTextBox
            {
                Text = "Offset Z",
                TooltipText = "The depth of the playfield.",
                DefaultText = move.OffsetZ.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        move.OffsetZ = result;
                    else
                        box.NotifyError();

                    Map.Update(move);
                }
            },
            new PointSettingsEasing<PlayfieldMoveEvent>(Map, move),
            new PointSettingsSlider<int>
            {
                Text = "Player Index",
                TooltipText = "The player to apply this to.",
                CurrentValue = move.PlayfieldIndex,
                Min = 0,
                Max = Map.MapInfo.IsDual ? 2 : 0,
                Step = 1,
                OnValueChanged = value =>
                {
                    move.PlayfieldIndex = value;
                    Map.Update(move);
                }
            },
            new PointSettingsSlider<int>
            {
                Text = "Subfield Index",
                TooltipText = "The subfield to apply this to.",
                CurrentValue = move.PlayfieldSubIndex,
                Min = 0,
                Max = Map.MapInfo.ExtraPlayfields + 1,
                Step = 1,
                OnValueChanged = value =>
                {
                    move.PlayfieldSubIndex = value;
                    Map.Update(move);
                }
            }
        });
    }
}
