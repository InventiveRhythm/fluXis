using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShaderEntry : PointListEntry
{
    protected override string Text => "Shader";
    protected override Colour4 Color => FluXisColors.Shader;

    private ShaderEvent shader => Object as ShaderEvent;

    private float maxStrength
    {
        get
        {
            if (shader.ShaderName == "Chromatic")
                return 20f;

            return 1f;
        }
    }

    private float step
    {
        get
        {
            if (shader.ShaderName == "Chromatic")
                return 1f;

            return .01f;
        }
    }

    public ShaderEntry(ShaderEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone()
    {
        return new ShaderEvent
        {
            Time = Object.Time,
            Duration = shader.Duration,
            ShaderName = shader.ShaderName,
            Parameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.Parameters.Strength
            }
        };
    }

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{shader.ShaderName} {(int)shader.Duration}ms ({shader.Parameters.Strength})",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<ShaderEvent>(Map, shader, BeatLength),
            new PointSettingsDropdown<string>
            {
                Text = "Shader",
                TooltipText = "The shader to apply to the playfield.",
                CurrentValue = shader.ShaderName,
                Items = ShaderEvent.ShaderNames.ToList(),
                OnValueChanged = value =>
                {
                    shader.ShaderName = value;
                    Map.Update(shader);
                }
            },
            new PointSettingsSlider<float>
            {
                Text = "Strength",
                TooltipText = "The strength of the shader effect.",
                CurrentValue = shader.Parameters.Strength,
                Min = 0,
                Max = maxStrength,
                Step = step,
                OnValueChanged = value =>
                {
                    shader.Parameters.Strength = value;
                    Map.Update(shader);
                }
            }
        });
    }
}
