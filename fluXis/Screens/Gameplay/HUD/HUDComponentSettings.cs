using System;
using System.Collections.Generic;
using System.Reflection;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD;

public class HUDComponentSettings
{
    [JsonIgnore]
    public string Key { get; set; } = string.Empty;

    [JsonIgnore]
    public Drawable Drawable { get; set; }

    public Vector2 Position { get; set; } = Vector2.Zero;
    public Anchor Anchor { get; set; } = Anchor.TopLeft;
    public Anchor Origin { get; set; } = Anchor.TopLeft;
    public float Scale { get; set; } = 1f;
    public bool AnchorToPlayfield { get; set; }

    public Dictionary<string, object> Settings { get; init; } = new();
    private bool settingsApplied;

    public void ApplyTo(Drawable drawable)
    {
        drawable.Anchor = Anchor;
        drawable.Origin = Origin;
        drawable.Position = Position;
        drawable.Scale = new Vector2(Scale);

        applySettings(drawable);
    }

    public void GetSettingsFrom(Drawable drawable)
    {
        var infos = drawable.GetSettingInfos();

        foreach (var info in infos)
        {
            if (string.IsNullOrWhiteSpace(info.Attribute.Key))
                continue; // maybe throw?

            var type = info.Bindable.GetType();
            var prop = type.GetProperty(nameof(Bindable<object>.Value), BindingFlags.Public | BindingFlags.Instance);

            if (prop is null)
                continue;

            var value = prop.GetValue(info.Bindable);
            Settings[info.Attribute.Key] = value;
        }
    }

    private void applySettings(Drawable drawable)
    {
        if (settingsApplied)
            return;

        settingsApplied = true;

        var infos = drawable.GetSettingInfos();

        foreach (var info in infos)
        {
            try
            {
                var key = info.Attribute.Key;

                if (string.IsNullOrWhiteSpace(key) || !Settings.TryGetValue(key, out var value))
                    continue;

                var type = info.Bindable.GetType();
                var prop = type.GetProperty(nameof(Bindable<object>.Value), BindingFlags.Public | BindingFlags.Instance);

                if (prop is null)
                    continue;

                if (prop.PropertyType == typeof(float) && value is double d)
                    value = (float)d;
                else if (prop.PropertyType == typeof(int) && value is double d2)
                    value = (int)d2;

                prop.SetValue(info.Bindable, value);
            }
            catch (Exception e)
            {
                Logger.Log($"Failed to apply setting '{info.Attribute.Key}' to {drawable.GetType().Name}: {e.Message}", LoggingTarget.Runtime, LogLevel.Error);
            }
        }
    }
}
