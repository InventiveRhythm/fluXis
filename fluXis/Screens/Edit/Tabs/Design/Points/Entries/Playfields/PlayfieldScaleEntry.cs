using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries.Playfields;

public partial class PlayfieldScaleEntry : PointListEntry
{
    protected override string Text => "Playfield Scale";
    protected override Colour4 Color => Theme.PlayfieldScale;

    private PlayfieldScaleEvent scale => Object as PlayfieldScaleEvent;

    public PlayfieldScaleEntry(PlayfieldScaleEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => scale.JsonCopy();

    protected override Drawable[] CreateValueContent() => new FluXisSpriteText
    {
        Text = $"{scale.ScaleX.ToStringInvariant("0.00")}x{scale.ScaleY.ToStringInvariant("0.00")} {(int)scale.Duration}ms P{scale.PlayfieldIndex}S{scale.PlayfieldSubIndex}",
        Colour = Color
    }.Yield().ToArray<Drawable>();

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableLength<PlayfieldScaleEvent>(Map, scale, BeatLength),
        new EditorVariableNumber<float>
        {
            Text = "ScaleX",
            TooltipText = "The horizontal scale of the playfield.",
            Formatting = "0.0##",
            CurrentValue = scale.ScaleX,
            Step = 0.05f,
            OnValueChanged = v =>
            {
                scale.ScaleX = v;
                Map.Update(scale);
            }
        },
        new EditorVariableNumber<float>
        {
            Text = "ScaleY",
            TooltipText = "The vertical scale of the playfield.",
            Formatting = "0.0##",
            CurrentValue = scale.ScaleY,
            Step = 0.05f,
            OnValueChanged = v =>
            {
                scale.ScaleY = v;
                Map.Update(scale);
            }
        },
        new EditorVariableEasing<PlayfieldScaleEvent>(Map, scale),
        new EditorVariableSlider<int>
        {
            Text = "Player Index",
            TooltipText = "The player to apply this to.",
            CurrentValue = scale.PlayfieldIndex,
            Min = 0,
            Max = Map.MapInfo.IsDual ? 2 : 0,
            Step = 1,
            OnValueChanged = value =>
            {
                scale.PlayfieldIndex = value;
                Map.Update(scale);
            }
        },
        new EditorVariableSlider<int>
        {
            Text = "Subfield Index",
            TooltipText = "The subfield to apply this to.",
            CurrentValue = scale.PlayfieldSubIndex,
            Min = 0,
            Max = Map.MapInfo.ExtraPlayfields + 1,
            Step = 1,
            OnValueChanged = value =>
            {
                scale.PlayfieldSubIndex = value;
                Map.Update(scale);
            }
        }
    });
}
