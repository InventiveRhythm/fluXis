using System;
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

public partial class LayerFadeEntry : PointListEntry
{
    protected override string Text => "Layer Fade";
    protected override Colour4 Color => FluXisColors.LayerFade;

    private LayerFadeEvent fade => Object as LayerFadeEvent;

    public LayerFadeEntry(LayerFadeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new LayerFadeEvent
    {
        Time = Object.Time,
        Duration = fade.Duration,
        Alpha = fade.Alpha,
        Easing = fade.Easing,
        Layer = fade.Layer
    };

    protected override Drawable[] CreateValueContent() => new Drawable[]
    {
        new FluXisSpriteText
        {
            Text = $"{fade.Layer.ToString()} {(fade.Alpha * 100).ToStringInvariant()}% {(int)fade.Duration}ms",
            Colour = Color
        }
    };

    protected override IEnumerable<Drawable> CreateSettings()
    {
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
                    Map.Update(fade);
                }
            }
        });
    }
}
