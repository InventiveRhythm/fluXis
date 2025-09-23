using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

public partial class DebugSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Bug;
    public override LocalisableString Title => strings.Title;

    private SettingsDebugStrings strings => LocalizationStrings.Settings.Debug;

    [BackgroundDependencyLoader(true)]
    private void load(FrameworkConfigManager frameworkConfig, FluXisConfig config, FluXisGameBase game, PanelContainer panels)
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.ShowLogOverlay,
                Bindable = frameworkConfig.GetBindable<bool>(FrameworkSetting.ShowLogOverlay)
            },
            new SettingsButton
            {
                Label = strings.ImportFile,
                ButtonText = "Import",
                Action = () =>
                {
                    panels.Content = new FileSelect
                    {
                        OnFileSelected = file => game.HandleDragDrop(file.FullName)
                    };
                }
            },
            new SettingsToggle
            {
                Label = strings.StreamFileBrowser,
                Description = strings.StreamFileBrowserDescription,
                Bindable = config.GetBindable<bool>(FluXisSetting.StreamFileBrowser)
            },
            new SettingsToggle
            {
                Label = strings.LogAPI,
                Description = strings.LogAPIDescription,
                Bindable = config.GetBindable<bool>(FluXisSetting.LogAPIResponses)
            },
            new SettingsToggle
            {
                Label = strings.ShowMissingLocalizations,
                Description = strings.ShowMissingLocalizationsDescription,
                Bindable = config.GetBindable<bool>(FluXisSetting.ShowMissingLocalizations)
            },
            new FluXisSpriteText
            {
                Text = "there is nothing down here... go back.",
                WebFontSize = 16,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Top = 1200 }
            },
            new FluXisSpriteText
            {
                Text = "i said go back.",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 60, Top = 800 }
            },
            new FluXisSpriteText
            {
                Text = "stop scrolling.",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 160, Top = 1500 }
            },
            new FluXisSpriteText
            {
                Text = "are you even listening???",
                WebFontSize = 16,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Right = 130, Top = 1900 }
            },
            new FluXisSpriteText
            {
                Text = "hello????",
                WebFontSize = 16,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Right = 320, Top = 1200 }
            },
            new FluXisSpriteText
            {
                Text = ".....",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 220, Top = 2200 }
            },
            new FluXisSpriteText
            {
                Text = "please?",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 320, Top = 1800 }
            },
            new FluXisSpriteText
            {
                Text = "guess not then.",
                WebFontSize = 16,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Right = 420, Top = 2800 }
            },
            new FluXisSpriteText
            {
                Text = "if you really want to know...",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 340, Top = 2200 }
            },
            new FluXisSpriteText
            {
                Text = "the answer is a loading screen text",
                WebFontSize = 20,
                Margin = new MarginPadding { Left = 380 }
            },
            new FluXisSpriteText
            {
                Text = "once you found it,",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 280, Top = 400 }
            },
            new FluXisSpriteText
            {
                Text = "come back here and use WASD to enter the code....",
                WebFontSize = 16,
                Margin = new MarginPadding { Left = 290, Top = 200 }
            },
            new FluXisSpriteText
            {
                Text = "if anybody asks... you heard nothing from me.",
                WebFontSize = 16,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Right = 290, Top = 100 }
            }
        });
    }
}
