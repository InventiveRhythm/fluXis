using System;
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
using fluXis.Utils.Attributes;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShaderEntry : PointListEntry
{
    protected override string Text => "Shader";
    protected override Colour4 Color => Theme.Shader;

    private ShaderEvent shader => Object as ShaderEvent;

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

                    var max = getMax(value);
                    shader.StartParameters.Strength = Math.Clamp(shader.StartParameters.Strength, 0, max.s1);
                    shader.StartParameters.Strength2 = Math.Clamp(shader.StartParameters.Strength2, 0, max.s2);
                    shader.StartParameters.Strength3 = Math.Clamp(shader.StartParameters.Strength3, 0, max.s3);
                    shader.EndParameters.Strength = Math.Clamp(shader.EndParameters.Strength, 0, max.s1);
                    shader.EndParameters.Strength2 = Math.Clamp(shader.EndParameters.Strength2, 0, max.s2);
                    shader.EndParameters.Strength3 = Math.Clamp(shader.EndParameters.Strength3, 0, max.s3);

                    Map.Update(shader);
                    OpenSettings();
                }
            },
            startValToggle
        };

        var attrs = shader.Type.TryGetAllAttributes<ShaderType, ShaderStrengthAttribute>(out var a) ? a : new ShaderStrengthAttribute[] { new() };

        foreach (var attribute in attrs)
        {
            if (attribute.Single)
            {
                settings.Add(new PointSettingsSlider<float>
                {
                    Text = attribute.ParamName ?? "Strength",
                    TooltipText = attribute.Tooltip ?? string.Empty,
                    CurrentValue = shader.StartParameters.Get(attribute.Index),
                    Min = attribute.Min,
                    Max = attribute.Max,
                    Step = attribute.Step,
                    OnValueChanged = value =>
                    {
                        shader.StartParameters.Set(attribute.Index, value);
                        shader.EndParameters.Set(attribute.Index, value);
                        Map.Update(shader);
                    }
                });
            }
            else
            {
                settings.AddRange(new Drawable[]
                {
                    new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = "Start " + (attribute.ParamName ?? "Strength"),
                        TooltipText = attribute.Tooltip ?? string.Empty,
                        CurrentValue = shader.StartParameters.Get(attribute.Index),
                        Min = attribute.Min,
                        Max = attribute.Max,
                        Step = attribute.Step,
                        OnValueChanged = value =>
                        {
                            shader.StartParameters.Set(attribute.Index, value);
                            Map.Update(shader);
                        }
                    },
                    new PointSettingsSlider<float>
                    {
                        Text = "End " + (attribute.ParamName ?? "Strength"),
                        TooltipText = attribute.Tooltip ?? string.Empty,
                        CurrentValue = shader.EndParameters.Get(attribute.Index),
                        Min = attribute.Min,
                        Max = attribute.Max,
                        Step = attribute.Step,
                        OnValueChanged = value =>
                        {
                            shader.EndParameters.Set(attribute.Index, value);
                            Map.Update(shader);
                        }
                    }
                });
            }
        }

        settings.Add(new PointSettingsEasing<ShaderEvent>(Map, shader));
        return base.CreateSettings().Concat(settings);
    }

    private (float s1, float s2, float s3) getMax(ShaderType type)
    {
        if (!type.TryGetAllAttributes<ShaderType, ShaderStrengthAttribute>(out var attrs))
            return (1, 1, 1);

        return (
            attrs.FirstOrDefault(x => x.Index == 1)?.Max ?? 1f,
            attrs.FirstOrDefault(x => x.Index == 2)?.Max ?? 1f,
            attrs.FirstOrDefault(x => x.Index == 3)?.Max ?? 1f
        );
    }
}
