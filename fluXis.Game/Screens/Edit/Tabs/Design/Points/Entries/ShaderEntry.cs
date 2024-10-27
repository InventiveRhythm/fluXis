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
using osu.Framework.Bindables;
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
            if (shader.Type == ShaderType.Chromatic)
                return 20f;

            return 1f;
        }
    }

    private float maxStrength2 => 1f;

    private float step
    {
        get
        {
            if (shader.Type == ShaderType.Chromatic)
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
            Type = shader.Type,
            UseStartParams = shader.UseStartParams,
            StartParameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.StartParameters.Strength,
                Strength2 = shader.StartParameters.Strength2,
            },
            EndParameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.EndParameters.Strength,
                Strength2 = shader.EndParameters.Strength2
            }
        };
    }

    protected override Drawable[] CreateValueContent()
    {
        var text = $"{shader.ShaderName} {(int)shader.Duration}ms (";

        if (shader.UseStartParams)
            text += $"{shader.StartParameters.Strength} > ";

        text += $"{shader.EndParameters.Strength})";

        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = text,
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var startValToggle = new PointSettingsToggle
        {
            Text = "Use Start Value",
            TooltipText = "Enables whether start values should be used.",
            Bindable = new Bindable<bool>(shader.UseStartParams),
            OnStateChanged = enabled =>
            {
                shader.UseStartParams = enabled;
                Map.Update(shader);
            }
        };

        var settings = new List<Drawable>
        {
            new PointSettingsLength<ShaderEvent>(Map, shader, BeatLength),
            new PointSettingsDropdown<ShaderType>
            {
                Text = "Shader",
                TooltipText = "The shader to apply to the playfield.",
                CurrentValue = shader.Type,
                Items = Enum.GetValues<ShaderType>().ToList(),
                OnValueChanged = value =>
                {
                    shader.Type = value;
                    Map.Update(shader);
                }
            },

            new PointSettingsToggle
            {
                Text = "Use Start Value",
                TooltipText = "Enables whether start values should be used.",
                Bindable = new Bindable<bool>(shader.UseStartParams),
                OnStateChanged = enabled =>
                {
                    shader.UseStartParams = enabled;
                    Map.Update(shader);
                }
            },

            new PointSettingsSlider<float>
            {
                Enabled = startValToggle.Bindable,
                Text = "Start Strength",
                TooltipText = "The strength of the shader effect.",
                CurrentValue = shader.StartParameters.Strength,
                Min = 0,
                Max = maxStrength,
                Step = step,
                OnValueChanged = value =>
                {
                    shader.StartParameters.Strength = value;
                    Map.Update(shader);
                }
            },

            new PointSettingsSlider<float>
            {
                Text = "End Strength",
                TooltipText = "The strength of the shader effect.",
                CurrentValue = shader.EndParameters.Strength,
                Min = 0,
                Max = maxStrength,
                Step = step,
                OnValueChanged = value =>
                {
                    shader.EndParameters.Strength = value;
                    Map.Update(shader);
                }
            }
        };

        // edge cases for shaders with extra parameters
        if (shader.Type == ShaderType.Glitch)
        {
            settings.AddRange(new Drawable[]
            {
                new PointSettingsSlider<float>
                {
                    Enabled = startValToggle.Bindable,
                    Text = "Start Block Size",
                    TooltipText = "The size of the glitch blocks.",
                    CurrentValue = shader.StartParameters.Strength2,
                    Min = 0,
                    Max = maxStrength2,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.StartParameters.Strength2 = value;
                        Map.Update(shader);
                    }
                },

                new PointSettingsSlider<float>
                {
                    Text = "End Block Size",
                    TooltipText = "The size of the glitch blocks.",
                    CurrentValue = shader.EndParameters.Strength2,
                    Min = 0,
                    Max = maxStrength2,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.EndParameters.Strength2 = value;
                        Map.Update(shader);
                    }
                }
            });
        }

        settings.Add(new PointSettingsEasing<ShaderEvent>(Map, shader));

        return base.CreateSettings().Concat(settings);
    }
}
