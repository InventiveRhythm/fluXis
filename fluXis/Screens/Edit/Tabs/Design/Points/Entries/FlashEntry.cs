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
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class FlashEntry : PointListEntry
{
    protected override string Text => "Flash";
    protected override Colour4 Color => Theme.Flash;

    private FlashEvent flash => Object as FlashEvent;

    public FlashEntry(FlashEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => flash.JsonCopy();

    protected override Drawable[] CreateValueContent() => new Drawable[]
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

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new EditorVariableLength<FlashEvent>(Map, flash, BeatLength),
        new EditorVariableToggle
        {
            Text = "In Background",
            TooltipText = "Whether the flash should be drawn behind the playfield and hud.",
            CurrentValue = flash.InBackground,
            OnValueChanged = b =>
            {
                flash.InBackground = b;
                Map.Update(flash);
            }
        },
        new EditorVariableSlider<float>
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
        new EditorVariableColor
        {
            Text = "Start Color",
            TooltipText = "The starting color of the flash.",
            CurrentValue = flash.StartColor,
            OnValueChanged = c =>
            {
                flash.StartColor = c;
                Map.Update(flash);
            }
        },
        new EditorVariableSlider<float>
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
        new EditorVariableColor
        {
            Text = "End Color",
            TooltipText = "The ending color of the flash.",
            CurrentValue = flash.EndColor,
            OnValueChanged = c =>
            {
                flash.EndColor = c;
                Map.Update(flash);
            }
        },
        new EditorVariableEasing<FlashEvent>(Map, flash)
    });
}
