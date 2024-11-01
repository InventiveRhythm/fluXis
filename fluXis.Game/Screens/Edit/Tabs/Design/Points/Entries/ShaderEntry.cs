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
                Strength3 = shader.StartParameters.Strength3
            },
            EndParameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.EndParameters.Strength,
                Strength2 = shader.EndParameters.Strength2,
                Strength3 = shader.EndParameters.Strength3
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
                    RequestClose?.Invoke(); // until there is a way to refresh
                    shader.Type = value;
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
