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

public partial class ColorFadeEntry : PointListEntry
{
    protected override string Text => "Color Fade";
    protected override Colour4 Color => Theme.ColorFade;

    private ColorFadeEvent colorFade => Object as ColorFadeEvent;

    public ColorFadeEntry(ColorFadeEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => colorFade.JsonCopy();

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new Circle
            {
                Size = new Vector2(20),
                Colour = colorFade.Primary,
                Margin = new MarginPadding { Right = 10 }
            },
            new Circle
            {
                Size = new Vector2(20),
                Colour = colorFade.Secondary,
                Margin = new MarginPadding { Right = 4 }
            },
            new Circle
            {
                Size = new Vector2(20),
                Colour = colorFade.Middle,
                Margin = new MarginPadding { Right = 10 }
            },
            new FluXisSpriteText
            {
                Text = $"{(int)colorFade.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<ColorFadeEvent>(Map, colorFade, BeatLength),
            new PointSettingsColor
            {
                Text = "Primary",
                TooltipText = "Color of the primary lane, right playfield border & Health top gradient.",
                Color = colorFade.Primary,
                OnColorChanged = c =>
                {
                    colorFade.Primary = c;
                    Map.Update(colorFade);
                }
            },
            new PointSettingsColor
            {
                Text = "Secondary",
                TooltipText = "Color of the secondary lane, left playfield border & Health Bottom gradient.",
                Color = colorFade.Secondary,
                OnColorChanged = c =>
                {
                    colorFade.Secondary = c;
                    Map.Update(colorFade);
                }
            },
            new PointSettingsColor
            {
                Text = "Middle",
                TooltipText = "Color of the middle lane.",
                Color = colorFade.Middle,
                OnColorChanged = c =>
                {
                    colorFade.Middle = c;
                    Map.Update(colorFade);
                }
            },
            new PointSettingsEasing<ColorFadeEvent>(Map, colorFade),
            new PointSettingsSlider<int>
            {
                Text = "Player Index",
                TooltipText = "The player to apply this to.",
                CurrentValue = colorFade.PlayfieldIndex,
                Min = 0,
                Max = Map.MapInfo.IsDual ? 2 : 0,
                Step = 1,
                OnValueChanged = value =>
                {
                    colorFade.PlayfieldIndex = value;
                    Map.Update(colorFade);
                }
            },
            new PointSettingsSlider<int>
            {
                Text = "Subfield Index",
                TooltipText = "The subfield to apply this to.",
                CurrentValue = colorFade.PlayfieldSubIndex,
                Min = 0,
                Max = Map.MapInfo.ExtraPlayfields + 1,
                Step = 1,
                OnValueChanged = value =>
                {
                    colorFade.PlayfieldSubIndex = value;
                    Map.Update(colorFade);
                }
            }
        });
    }
}
