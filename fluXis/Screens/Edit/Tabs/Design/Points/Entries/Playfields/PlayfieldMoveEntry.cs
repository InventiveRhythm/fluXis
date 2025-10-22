using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Playfields;

public partial class PlayfieldMoveEntry : PointListEntry
{
    protected override string Text => "Playfield Move";
    protected override Colour4 Color => Theme.PlayfieldMove;

    private PlayfieldMoveEvent move => Object as PlayfieldMoveEvent;

    public PlayfieldMoveEntry(PlayfieldMoveEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => move.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{(int)move.OffsetX}x {(int)move.OffsetY}y {(int)move.OffsetZ}z {(int)move.Duration}ms P{move.PlayfieldIndex}S{move.PlayfieldSubIndex}",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PlayfieldMoveEvent>(Map, move, BeatLength),
            new PointSettingsNumber<float>
            {
                Text = "Offset X",
                TooltipText = "The horizontal offset of the playfield.",
                DefaultValue = move.OffsetX,
                Step = 10,
                OnValueChanged = v =>
                {
                    move.OffsetX = v;
                    Map.Update(move);
                }
            },
            new PointSettingsNumber<float>
            {
                Text = "Offset Y",
                TooltipText = "The vertical offset of the playfield.",
                DefaultValue = move.OffsetY,
                Step = 10,
                OnValueChanged = v =>
                {
                    move.OffsetY = v;
                    Map.Update(move);
                }
            },
            new PointSettingsNumber<float>
            {
                Text = "Offset Z",
                TooltipText = "The depth of the playfield.",
                DefaultValue = move.OffsetZ,
                Step = 10,
                OnValueChanged = v =>
                {
                    move.OffsetZ = v;
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
