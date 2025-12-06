using System;
using System.Numerics;
using fluXis.Configuration;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using Vector2 = osuTK.Vector2;

namespace fluXis.Screens.Select.Info.Tabs.Settings;

public partial class MapSettingsTab : SelectInfoTab
{
    public override LocalisableString Title => "Settings";
    public override IconUsage Icon => FontAwesome6.Solid.Gear;
    public override bool ShowBackdrop => true;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = true,
                ScrollbarOverlapsContent = false,
                Child = flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(32) { Vertical = 16 },
                    Spacing = new Vector2(8)
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(onMapChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= onMapChanged;
    }

    private void onMapChanged(ValueChangedEvent<RealmMap> e)
    {
        flow.Clear();

        flow.Add(new FluXisSpriteText
        {
            Text = "These settings only apply to the current map.",
            WebFontSize = 14,
            Alpha = 0.6f
        });

        var settings = e.NewValue.Settings;

        createSlider("Audio offset", "Higher means objects appear later.", v => update(s => s.Offset = v), settings.Offset, 0, 1, -50, 50);
        createToggle("Disable colors", "Disables the custom lane colors.", v => update(s => s.DisableColors = v), settings.DisableColors, false);
        createToggle("Disable hit sounds", "Disables the custom sounds when hitting notes.", v => update(s => s.DisableHitSounds = v), settings.DisableHitSounds, false);

        var scrollSpeed = config.Get<float>(FluXisSetting.ScrollSpeed);
        createSlider("Scroll speed", "", v => update(s => s.ScrollSpeed = Math.Abs(v - scrollSpeed) < 0.1f ? null : v), settings.ScrollSpeed ?? scrollSpeed, scrollSpeed, 0.1f, 2, 8);
        return;

        void update(Action<RealmMapUserSettings> action) => realm.RunWrite(r =>
        {
            var map = r.Find<RealmMap>(e.NewValue.ID);

            action?.Invoke(settings);
            action?.Invoke(map.Settings);
        });
    }

    private void createSlider<T>(LocalisableString title, LocalisableString description, Action<T> change, T value, T def, T step, T min, T max)
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
        var bind = new BindableNumber<T>
        {
            Value = value,
            Default = def,
            Precision = step,
            MinValue = min,
            MaxValue = max
        };

        bind.ValueChanged += v => change?.Invoke(v.NewValue);

        var setting = new SettingsSlider<T>
        {
            Label = title,
            Description = description,
            Step = step,
            Bindable = bind,
            SmallReset = true
        };

        flow.Add(setting);
    }

    private void createToggle(LocalisableString title, LocalisableString description, Action<bool> change, bool value, bool def)
    {
        var bind = new BindableBool()
        {
            Value = value,
            Default = def
        };

        bind.ValueChanged += v => change?.Invoke(v.NewValue);

        var setting = new SettingsToggle()
        {
            Label = title,
            Description = description,
            Bindable = bind,
            SmallReset = true
        };

        flow.Add(setting);
    }
}
