using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class FlashEntry : PointListEntry
{
    protected override string Text => "Flash";
    protected override Colour4 Color => Colour4.FromHex("#ffffff");

    private FlashEvent flash => Object as FlashEvent;

    public FlashEntry(FlashEvent obj)
        : base(obj)
    {
    }

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
                Text = $"{flash.Duration}ms",
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
                Color = flash.EndColor,
                OnColorChanged = c =>
                {
                    flash.EndColor = c;
                    Map.Update(flash);
                }
            },
        });
    }
}
