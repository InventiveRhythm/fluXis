using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Screens.Edit.UI.Variable.Preset;
using fluXis.Utils;
using osu.Framework.Bindables;
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

    protected override Drawable[] CreateValueContent() => new Drawable[]
    {
        new Circle
        {
            Size = new Vector2(20),
            Colour = colorFade.Primary,
            Alpha = colorFade.FadePrimary ? 1f : .4f,
            Margin = new MarginPadding { Right = 4 }
        },
        new Circle
        {
            Size = new Vector2(20),
            Colour = colorFade.Secondary,
            Alpha = colorFade.FadeSecondary ? 1f : .4f,
            Margin = new MarginPadding { Right = 4 }
        },
        new Circle
        {
            Size = new Vector2(20),
            Colour = colorFade.Middle,
            Alpha = colorFade.FadeMiddle ? 1f : .4f,
            Margin = new MarginPadding { Right = 10 }
        },
        new FluXisSpriteText
        {
            Text = $"{(int)colorFade.Duration}ms P{colorFade.PlayfieldIndex}S{colorFade.PlayfieldSubIndex}",
            Colour = Color
        }
    };

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var primaryToggle = new EditorVariableToggle
        {
            Text = "Fade Primary",
            TooltipText = "Enables whether Primary color should be faded.",
            Bindable = new Bindable<bool>(colorFade.FadePrimary),
            OnValueChanged = enabled =>
            {
                colorFade.FadePrimary = enabled;
                Map.Update(colorFade);
            }
        };

        var secondaryToggle = new EditorVariableToggle
        {
            Text = "Fade secondary",
            TooltipText = "Enables whether secondary color should be faded.",
            Bindable = new Bindable<bool>(colorFade.FadeSecondary),
            OnValueChanged = enabled =>
            {
                colorFade.FadeSecondary = enabled;
                Map.Update(colorFade);
            }
        };

        var middleToggle = new EditorVariableToggle
        {
            Text = "Fade middle",
            TooltipText = "Enables whether middle color should be faded.",
            Bindable = new Bindable<bool>(colorFade.FadeMiddle),
            OnValueChanged = enabled =>
            {
                colorFade.FadeMiddle = enabled;
                Map.Update(colorFade);
            }
        };

        return base.CreateSettings().Concat(new Drawable[]
        {
            new EditorVariableLength<ColorFadeEvent>(Map, colorFade, BeatLength),
            primaryToggle,
            new EditorVariableColor
            {
                Enabled = primaryToggle.Bindable,
                Text = "Primary",
                TooltipText = "Color of the primary lane, left stage border & Health top gradient.",
                CurrentValue = colorFade.Primary,
                OnValueChanged = c =>
                {
                    colorFade.Primary = c;
                    Map.Update(colorFade);
                }
            },
            secondaryToggle,
            new EditorVariableColor
            {
                Enabled = secondaryToggle.Bindable,
                Text = "Secondary",
                TooltipText = "Color of the secondary lane, right stage border & Health Bottom gradient.",
                CurrentValue = colorFade.Secondary,
                OnValueChanged = c =>
                {
                    colorFade.Secondary = c;
                    Map.Update(colorFade);
                }
            },
            middleToggle,
            new EditorVariableColor
            {
                Enabled = middleToggle.Bindable,
                Text = "Middle",
                TooltipText = "Color of the middle lane.",
                CurrentValue = colorFade.Middle,
                OnValueChanged = c =>
                {
                    colorFade.Middle = c;
                    Map.Update(colorFade);
                }
            },
            new EditorVariableEasing<ColorFadeEvent>(Map, colorFade),
            new EditorVariableSlider<int>
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
            new EditorVariableSlider<int>
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
