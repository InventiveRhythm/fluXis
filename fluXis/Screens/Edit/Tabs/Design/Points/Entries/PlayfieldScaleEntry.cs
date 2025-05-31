using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class PlayfieldScaleEntry : PointListEntry
{
    protected override string Text => "Playfield Scale";
    protected override Colour4 Color => FluXisColors.PlayfieldScale;

    private PlayfieldScaleEvent scale => Object as PlayfieldScaleEvent;

    public PlayfieldScaleEntry(PlayfieldScaleEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => scale.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{scale.ScaleX.ToStringInvariant("0.00")}x{scale.ScaleY.ToStringInvariant("0.00")} {(int)scale.Duration}ms P{scale.PlayfieldIndex}S{scale.PlayfieldSubIndex}",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<PlayfieldScaleEvent>(Map, scale, BeatLength),
            new PointSettingsTextBox
            {
                Text = "ScaleX",
                TooltipText = "The horizontal scale of the playfield.",
                DefaultText = scale.ScaleX.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        scale.ScaleX = result;
                    else
                        box.NotifyError();

                    Map.Update(scale);
                }
            },
            new PointSettingsTextBox
            {
                Text = "ScaleY",
                TooltipText = "The vertical scale of the playfield.",
                DefaultText = scale.ScaleY.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseFloatInvariant(out var result))
                        scale.ScaleY = result;
                    else
                        box.NotifyError();

                    Map.Update(scale);
                }
            },
            new PointSettingsEasing<PlayfieldScaleEvent>(Map, scale),
            new PointSettingsSlider<int>
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
            new PointSettingsSlider<int>
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
}
