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

public partial class PlayfieldRotateEntry : PointListEntry
{
    protected override string Text => "Playfield Rotate";
    protected override Colour4 Color => Theme.PlayfieldRotate;

    private PlayfieldRotateEvent rotate => Object as PlayfieldRotateEvent;

    public PlayfieldRotateEntry(PlayfieldRotateEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => rotate.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{(int)rotate.Roll}deg {(int)rotate.Duration}ms P{rotate.PlayfieldIndex}S{rotate.PlayfieldSubIndex}",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsLength<PlayfieldRotateEvent>(Map, rotate, BeatLength),
        new PointSettingsNumber<float>
        {
            Text = "Rotation",
            TooltipText = "The rotation of the playfield.",
            DefaultValue = rotate.Roll,
            Step = 2,
            OnValueChanged = v =>
            {
                rotate.Roll = v;
                Map.Update(rotate);
            }
        },
        new PointSettingsEasing<PlayfieldRotateEvent>(Map, rotate),
        new PointSettingsSlider<int>
        {
            Text = "Player Index",
            TooltipText = "The player to apply this to.",
            CurrentValue = rotate.PlayfieldIndex,
            Min = 0,
            Max = Map.MapInfo.IsDual ? 2 : 0,
            Step = 1,
            OnValueChanged = value =>
            {
                rotate.PlayfieldIndex = value;
                Map.Update(rotate);
            }
        },
        new PointSettingsSlider<int>
        {
            Text = "Subfield Index",
            TooltipText = "The subfield to apply this to.",
            CurrentValue = rotate.PlayfieldSubIndex,
            Min = 0,
            Max = Map.MapInfo.ExtraPlayfields + 1,
            Step = 1,
            OnValueChanged = value =>
            {
                rotate.PlayfieldSubIndex = value;
                Map.Update(rotate);
            }
        }
    });
}
