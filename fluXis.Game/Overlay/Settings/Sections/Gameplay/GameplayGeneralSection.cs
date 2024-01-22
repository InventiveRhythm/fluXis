using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayGeneralSection : SettingsSubSection
{
    public override string Title => "General";
    public override IconUsage Icon => FontAwesome6.Solid.Gear;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<ScrollDirection>
            {
                Label = "Scroll Direction",
                Items = Enum.GetValues<ScrollDirection>(),
                Bindable = Config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection)
            },
            new SettingsToggle
            {
                Label = "Snap Coloring",
                Description = "Color notes based on their snap divisor.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.SnapColoring)
            },
            new SettingsToggle
            {
                Label = "Timing Lines",
                Description = "Show a line every 4 beats.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.TimingLines)
            },
            new SettingsToggle
            {
                Label = "Hide Flawless Judgement",
                Description = "Hides the best judgement (Flawless) from popping up during gameplay.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.HideFlawless)
            },
            new SettingsToggle
            {
                Label = "Show Early/Late",
                Description = "Show early/late below the judgement.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowEarlyLate)
            },
            new SettingsToggle
            {
                Label = "Judgement Splash",
                Description = "Show a splash when a judgement is hit.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.JudgementSplash)
            },
            new SettingsSlider<float>
            {
                Label = "Top Lane Cover",
                Bindable = Config.GetBindable<float>(FluXisSetting.LaneCoverTop),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = "Bottom Lane Cover",
                Bindable = Config.GetBindable<float>(FluXisSetting.LaneCoverBottom),
                DisplayAsPercentage = true
            },
            new SettingsToggle
            {
                Label = "Health Effects",
                Description = "Dims and fades the entire playfield when health is low.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.DimAndFade)
            }
        });
    }
}
