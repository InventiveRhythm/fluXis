using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Screens.Gameplay.HUD.Components;
using fluXis.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Screens.Gameplay.HUD;

public partial class LayoutManager : Component
{
    public Bindable<HUDLayout> Layout { get; } = new(new DefaultLayout());
    public List<HUDLayout> Layouts { get; } = new();

    public bool IsDefault => Layout.Value is DefaultLayout;

    public event Action Reloaded;

    [Resolved]
    private FluXisConfig config { get; set; }

    private readonly Dictionary<string, Type> componentLookup = new();
    public IDictionary<string, Type> ComponentTypes => componentLookup;

    private Storage storage;
    private Bindable<string> layoutName;

    [BackgroundDependencyLoader]
    private void load(Storage baseStorage)
    {
        componentLookup.Add("Accuracy", typeof(AccuracyDisplay));
        componentLookup.Add("AttributeText", typeof(AttributeText));
        componentLookup.Add("Combo", typeof(ComboCounter));
        componentLookup.Add("Health", typeof(HealthBar));
        componentLookup.Add("HitError", typeof(HitErrorBar));
        componentLookup.Add("Judgement", typeof(JudgementDisplay));
        componentLookup.Add("JudgementCounter", typeof(JudgementCounter));
        componentLookup.Add("PerformanceRating", typeof(PerformanceRatingDisplay));
        componentLookup.Add("Progress", typeof(Progressbar));

        storage = baseStorage.GetStorageForDirectory("layouts");
        layoutName = config.GetBindable<string>(FluXisSetting.LayoutName);
        Reload();

        Layout.BindValueChanged(e =>
        {
            layoutName.Value = e.NewValue != null ? e.NewValue.ID : "Default";
        }, true);
    }

    public void Reload()
    {
        Layouts.Clear();
        Layouts.Add(Layout.Default);

        loadLayouts();
        Layout.Value = Layouts.FirstOrDefault(x => x.ID == layoutName.Value) ?? Layouts.First();

        Reloaded?.Invoke();
    }

    public void CreateNewLayout(bool open = true)
    {
        var def = new DefaultLayout();
        var id = Guid.NewGuid().ToString();

        var layout = new HUDLayout
        {
            Name = "New Layout",
            Gameplay = def.Gameplay,
            ID = id
        };

        var path = storage.GetFullPath($"{id}.json");
        File.WriteAllText(path, layout.Serialize(true));

        Layouts.Add(layout);
        Layout.Value = layout;
        Reloaded?.Invoke();

        if (open)
            storage.PresentFileExternally(path);
    }

    public void PresentExternally()
    {
        var current = Layout.Value;

        if (current is DefaultLayout)
        {
            storage.PresentExternally();
            return;
        }

        var path = storage.GetFullPath($"{Layout.Value.ID}.json");
        storage.PresentFileExternally(path);
    }

    public GameplayHUDComponent CreateComponent(string key, HUDComponentSettings settings, IHUDDependencyProvider provider)
    {
        if (!componentLookup.TryGetValue(key, out var type))
            throw new ArgumentOutOfRangeException($"'{nameof(key)}' is not a valid component key.");

        var component = (GameplayHUDComponent)Activator.CreateInstance(type);

        if (component == null)
            throw new Exception($"Failed to create instance of {type}.");

        component.Populate(settings, provider);
        settings.ApplyTo(component);
        return component;
    }

    private void loadLayouts()
    {
        var files = storage.GetFiles(".", "*.json");

        foreach (var file in files)
        {
            try
            {
                var path = storage.GetFullPath(file);
                var layout = File.ReadAllText(path).Deserialize<HUDLayout>();

                layout.ID = Path.GetFileNameWithoutExtension(path);
                Layouts.Add(layout);
            }
            catch (Exception ex)
            {
                if (ex is JsonReaderException)
                    Logger.Log($"Failed to parse layout '{Path.GetFileName(file)}'! {ex.Message}", LoggingTarget.Runtime, LogLevel.Error);
                else
                    Logger.Error(ex, $"Failed to load layout {Path.GetFileName(file)}!");
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
                            { "size", 32d },
                            { "max-width", 512d }
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
                            { "text", "by {value}" },
                            { "size", 24d },
                            { "max-width", 512d }
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
                            { "size", 32d },
                            { "max-width", 512d }
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
                            { "text", "mapped by {value}" },
                            { "size", 24d },
                            { "max-width", 512d }
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
                        Position = new Vector2(0, -32),
                        Settings = new Dictionary<string, object>
                        {
                            { "scale-additive", true }
                        }
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
