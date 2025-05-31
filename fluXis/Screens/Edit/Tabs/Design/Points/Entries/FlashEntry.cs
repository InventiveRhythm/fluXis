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
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class FlashEntry : PointListEntry
{
    protected override string Text => "Flash";
    protected override Colour4 Color => FluXisColors.Flash;

    private FlashEvent flash => Object as FlashEvent;

    public FlashEntry(FlashEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => flash.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new Circle
            {
                Size = new Vector2(20),
                Colour = flash.StartColor,
                Margin = new MarginPadding { Right = 4 }
            },
            new Circle
            {
                Size = new Vector2(20),
                Colour = flash.EndColor,
                Margin = new MarginPadding { Right = 10 }
            },
            new FluXisSpriteText
            {
                Text = $"{(int)flash.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<FlashEvent>(Map, flash, BeatLength),
            new PointSettingsToggle
            {
                Text = "In Background",
                TooltipText = "Whether the flash should be drawn behind the playfield and hud.",
                CurrentValue = flash.InBackground,
                OnStateChanged = b =>
                {
                    flash.InBackground = b;
                    Map.Update(flash);
                }
            },
            new PointSettingsSlider<float>
            {
                Text = "Start Alpha",
                TooltipText = "The starting opacity of the flash.",
                CurrentValue = flash.StartOpacity,
                Step = .01f,
                Min = 0,
                Max = 1,
                OnValueChanged = v =>
                {
                    flash.StartOpacity = v;
                    Map.Update(flash);
                }
            },
            new PointSettingsColor
            {
                Text = "Start Color",
                TooltipText = "The starting color of the flash.",
                Color = flash.StartColor,
                OnColorChanged = c =>
                {
                    flash.StartColor = c;
                    Map.Update(flash);
                }
            },
            new PointSettingsSlider<float>
            {
                Text = "End Alpha",
                TooltipText = "The ending opacity of the flash.",
                CurrentValue = flash.EndOpacity,
                Step = .01f,
                Min = 0,
                Max = 1,
                OnValueChanged = v =>
                {
                    flash.EndOpacity = v;
                    Map.Update(flash);
                }
            },
            new PointSettingsColor
            {
                Text = "End Color",
                TooltipText = "The ending color of the flash.",
                Color = flash.EndColor,
                OnColorChanged = c =>
                {
                    flash.EndColor = c;
                    Map.Update(flash);
                }
            },
            new PointSettingsEasing<FlashEvent>(Map, flash)
        });
    }
}
