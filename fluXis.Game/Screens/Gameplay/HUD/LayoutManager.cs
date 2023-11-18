using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Screens.Gameplay.HUD.Components;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class LayoutManager : Component
{
    public Bindable<HUDLayout> Layout { get; } = new(new DefaultLayout());
    public List<HUDLayout> Layouts { get; } = new();

    public event Action Reloaded;

    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    private Bindable<string> layoutName;

    [BackgroundDependencyLoader]
    private void load()
    {
        layoutName = config.GetBindable<string>(FluXisSetting.LayoutName);
        Reload();

        Layout.BindValueChanged(e =>
        {
            layoutName.Value = e.NewValue != null ? e.NewValue.ID : "Default";
            Logger.Log($"Switched to layout {layoutName.Value}", LoggingTarget.Runtime, LogLevel.Debug);
        }, true);
    }

    public void Reload()
    {
        Layouts.Clear();
        Layouts.Add(new DefaultLayout());

        loadLayouts();
        Layout.Value = Layouts.FirstOrDefault(x => x.ID == layoutName.Value) ?? Layouts.First();

        Reloaded?.Invoke();
    }

    public void CreateNewLayout()
    {
        var id = Guid.NewGuid().ToString();

        var layout = new HUDLayout
        {
            Name = "New Layout",
            Gameplay = new DefaultLayout().Gameplay,
            ID = id
        };

        var path = Path.Combine(storage.GetFullPath("layouts"), $"{id}.json");
        var json = JsonConvert.SerializeObject(layout, Formatting.Indented);
        File.WriteAllText(path, json);

        Layouts.Add(layout);
        Layout.Value = layout;
        Reloaded?.Invoke();

        PathUtils.ShowFile(path);
    }

    private void loadLayouts()
    {
        var dirPath = storage.GetFullPath("layouts");

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        var files = Directory.GetFiles(dirPath, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var layout = JsonConvert.DeserializeObject<HUDLayout>(json);

                layout.ID = Path.GetFileNameWithoutExtension(file);
                Layouts.Add(layout);

                Logger.Log($"Loaded layout {layout.ID}", LoggingTarget.Runtime, LogLevel.Debug);
            }
            catch
            {
                Logger.Log($"Failed to load layout {Path.GetFileName(file)}", LoggingTarget.Runtime, LogLevel.Error);
            }
        }
    }

    public class DefaultLayout : HUDLayout
    {
        public DefaultLayout()
        {
            Name = "Default";
            Gameplay = new Dictionary<string, HUDComponentSettings>
            {
                {
                    "Accuracy",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre,
                        AnchorToPlayfield = true
                    }
                },
                {
                    "AttributeText#Title",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Position = new Vector2(20, -10),
                        Settings = new Dictionary<string, object>
                        {
                            { "type", AttributeType.Title },
                            { "size", 48d }
                        }
                    }
                },
                {
                    "AttributeText#Artist",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Position = new Vector2(20, -52),
                        Settings = new Dictionary<string, object>
                        {
                            { "type", AttributeType.Artist },
                            { "text", "by {value}" }
                        }
                    }
                },
                {
                    "AttributeText#Difficulty",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Position = new Vector2(-20, -10),
                        Settings = new Dictionary<string, object>
                        {
                            { "type", AttributeType.Difficulty },
                            { "size", 48d }
                        }
                    }
                },
                {
                    "AttributeText#Mapper",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Position = new Vector2(-20, -50),
                        Settings = new Dictionary<string, object>
                        {
                            { "type", AttributeType.Mapper },
                            { "text", "mapped by {value}" }
                        }
                    }
                },
                {
                    "Combo",
                    new HUDComponentSettings
                    {
                        AnchorToPlayfield = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Position = new Vector2(0, -32)
                    }
                },
                {
                    "Health",
                    new HUDComponentSettings
                    {
                        AnchorToPlayfield = true,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomLeft,
                        Position = new Vector2(20, -40)
                    }
                },
                {
                    "HitError",
                    new HUDComponentSettings
                    {
                        AnchorToPlayfield = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre,
                        Position = new Vector2(0, 50)
                    }
                },
                {
                    "Judgement",
                    new HUDComponentSettings
                    {
                        AnchorToPlayfield = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Position = new Vector2(0, 150)
                    }
                },
                {
                    "JudgementCounter",
                    new HUDComponentSettings
                    {
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Position = new Vector2(-20, 0)
                    }
                },
                {
                    "Progress",
                    new HUDComponentSettings()
                }
            };
        }

        public override string ToString() => "Default";
    }
}
