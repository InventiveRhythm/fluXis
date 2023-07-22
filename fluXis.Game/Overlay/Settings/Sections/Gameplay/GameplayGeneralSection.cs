using System;
using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayGeneralSection : SettingsSubSection
{
    public override string Title => "General";

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
                Description = "Color notes based on their snap divisor",
                Bindable = Config.GetBindable<bool>(FluXisSetting.SnapColoring)
            },
            new SettingsToggle
            {
                Label = "Hide Flawless Judgement",
                Bindable = Config.GetBindable<bool>(FluXisSetting.HideFlawless)
            },
            new SettingsToggle
            {
                Label = "Show Early/Late",
                Description = "Show early/late below the judgement",
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowEarlyLate)
            },
            new SettingsToggle
            {
                Label = "Judgement Splash",
                Description = "Show a splash when a judgement is hit",
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
                Label = "Dim and fade playfield to red when health is low",
                Bindable = Config.GetBindable<bool>(FluXisSetting.DimAndFade)
            }
        });
    }
}
