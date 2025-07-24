using System;
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
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class LayerFadeEntry : PointListEntry
{
    protected override string Text => "Layer Fade";
    protected override Colour4 Color => Theme.LayerFade;

    private LayerFadeEvent fade => Object as LayerFadeEvent;

    public LayerFadeEntry(LayerFadeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => fade.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        var str = $"{fade.Layer.ToString()} {(fade.Alpha * 100).ToStringInvariant()}% {(int)fade.Duration}ms";

        if (fade.Layer == LayerFadeEvent.FadeLayer.Playfield)
            str += $" P{fade.PlayfieldIndex}S{fade.PlayfieldSubIndex}";

        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = str,
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var layerPlayfield = new BindableBool(fade.Layer == LayerFadeEvent.FadeLayer.Playfield);

        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<LayerFadeEvent>(Map, fade, BeatLength),
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
            },
            new PointSettingsEasing<LayerFadeEvent>(Map, fade),
            new PointSettingsDropdown<LayerFadeEvent.FadeLayer>
            {
                Text = "Layer",
                TooltipText = "The layer to adjust the opacity of.",
                CurrentValue = fade.Layer,
                Items = Enum.GetValues<LayerFadeEvent.FadeLayer>().ToList(),
                OnValueChanged = value =>
                {
                    fade.Layer = value;
                    layerPlayfield.Value = value == LayerFadeEvent.FadeLayer.Playfield;
                    Map.Update(fade);
                }
            },
            new PointSettingsSlider<int>
            {
                Text = "Player Index",
                TooltipText = "The player to apply this to.",
                CurrentValue = fade.PlayfieldIndex,
                Min = 0,
                Max = Map.MapInfo.IsDual ? 2 : 0,
                Step = 1,
                Enabled = layerPlayfield,
                HideWhenDisabled = true,
                OnValueChanged = value =>
                {
                    fade.PlayfieldIndex = value;
                    Map.Update(fade);
                }
            },
            new PointSettingsSlider<int>
            {
                Text = "Subfield Index",
                TooltipText = "The subfield to apply this to.",
                CurrentValue = fade.PlayfieldSubIndex,
                Min = 0,
                Max = Map.MapInfo.ExtraPlayfields + 1,
                Step = 1,
                Enabled = layerPlayfield,
                HideWhenDisabled = true,
                OnValueChanged = value =>
                {
                    fade.PlayfieldSubIndex = value;
                    Map.Update(fade);
                }
            }
        });
    }
}
