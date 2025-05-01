using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
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

    private float maxStrength3 => 1f;

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

    public override ITimedObject CreateClone() => shader.JsonCopy();

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
                    RequestClose?.Invoke(); // until there is a way to refresh
                    shader.Type = value;
                    shader.StartParameters.Strength = Math.Clamp(shader.StartParameters.Strength, 0, maxStrength);
                    shader.StartParameters.Strength2 = Math.Clamp(shader.StartParameters.Strength2, 0, maxStrength2);
                    shader.StartParameters.Strength3 = Math.Clamp(shader.StartParameters.Strength3, 0, maxStrength3);
                    shader.EndParameters.Strength = Math.Clamp(shader.EndParameters.Strength, 0, maxStrength);
                    shader.EndParameters.Strength2 = Math.Clamp(shader.EndParameters.Strength2, 0, maxStrength2);
                    shader.EndParameters.Strength3 = Math.Clamp(shader.EndParameters.Strength3, 0, maxStrength3);
                    Map.Update(shader);
                    OpenSettings();
                }
            },
            startValToggle
        };

        // edge cases for shaders with extra/different parameter(s)
        switch (shader.Type)
        {
            case ShaderType.Glitch:
                settings.AddRange(new Drawable[]
                {
                    new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = "Start X Strength",
                        TooltipText = "The strength of the glitch effect on the x-axis.",
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
                        Text = "End X Strength",
                        TooltipText = "The strength of the glitch effect on the x-axis.",
                        CurrentValue = shader.EndParameters.Strength,
                        Min = 0,
                        Max = maxStrength,
                        Step = step,
                        OnValueChanged = value =>
                        {
                            shader.EndParameters.Strength = value;
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = "Start Y Strength",
                        TooltipText = "The strength of the glitch effect on the y-axis.",
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
                        Text = "End Y Strength",
                        TooltipText = "The strength of the glitch effect on the y-axis.",
                        CurrentValue = shader.EndParameters.Strength2,
                        Min = 0,
                        Max = maxStrength2,
                        Step = step,
                        OnValueChanged = value =>
                        {
                            shader.EndParameters.Strength2 = value;
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = "Start Block Size",
                        TooltipText = "The size of the glitch blocks.",
                        CurrentValue = shader.StartParameters.Strength3,
                        Min = 0,
                        Max = maxStrength3,
                        Step = step,
                        OnValueChanged = value =>
                        {
                            shader.StartParameters.Strength3 = value;
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Text = "End Block Size",
                        TooltipText = "The size of the glitch blocks.",
                        CurrentValue = shader.EndParameters.Strength3,
                        Min = 0,
                        Max = maxStrength3,
                        Step = step,
                        OnValueChanged = value =>
                        {
                            shader.EndParameters.Strength3 = value;
                            Map.Update(shader);
                        }
                    }
                });
                break;
            case ShaderType.SplitScreen:
                settings.AddRange(new Drawable[]
                {
                    new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = "Start Strength",
                        TooltipText = "The strength of the screen split.",
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
                        TooltipText = "The strength of the screen split.",
                        CurrentValue = shader.EndParameters.Strength,
                        Min = 0,
                        Max = maxStrength,
                        Step = step,
                        OnValueChanged = value =>
                        {
                            shader.EndParameters.Strength = value;
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Text = "Splits X",
                        TooltipText = "Splits on X axis",
                        CurrentValue = shader.StartParameters.Strength2,
                        Min = 1.0f,
                        Max = 16.0f,
                        Step = 1.0f,
                        OnValueChanged = value =>
                        {
                            shader.StartParameters.Strength2 = value;
                            shader.EndParameters.Strength2 = value;
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Text = "Splits Y",
                        TooltipText = "Splits on Y axis",
                        CurrentValue = shader.StartParameters.Strength3,
                        Min = 1.0f,
                        Max = 16.0f,
                        Step = 1.0f,
                        OnValueChanged = value =>
                        {
                            shader.StartParameters.Strength3 = value;
                            shader.EndParameters.Strength3 = value;
                            Map.Update(shader);
                        }
                    }
                });
                break;
            case ShaderType.Bloom:
            case ShaderType.Greyscale:
            case ShaderType.Invert:
            case ShaderType.Chromatic:
            case ShaderType.Mosaic:
            case ShaderType.Noise:
            case ShaderType.Vignette:
            case ShaderType.Retro:
            case ShaderType.HueShift:
            default: // default shader settings
                settings.AddRange(new Drawable[]
                {
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
                });
                break;
        }

        settings.Add(new PointSettingsEasing<ShaderEvent>(Map, shader));

        return base.CreateSettings().Concat(settings);
    }
}
