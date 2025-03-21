using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Appearance;

public partial class AppearanceSkinSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Skin;
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [Resolved]
    private SkinManager skinManager { get; set; }

    private SettingsDropdown<string> currentDropdown;
    private BindableBool buttonsEnabled;

    [BackgroundDependencyLoader(true)]
    private void load(SkinManager skinManager, [CanBeNull] FluXisGame game, PanelContainer panels)
    {
        buttonsEnabled = new BindableBool(true);

        AddRange(new Drawable[]
        {
            currentDropdown = new SettingsDropdown<string>
            {
                Label = strings.SkinCurrent,
                Description = strings.SkinCurrentDescription,
                Bindable = Config.GetBindable<string>(FluXisSetting.SkinName),
                Items = skinManager.GetSkinNames()
            },
            new SettingsButton
            {
                Label = strings.SkinRefresh,
                Description = strings.SkinRefreshDescription,
                ButtonText = "Refresh",
                Action = reloadList
            },
            new SettingsButton
            {
                Label = strings.SkinOpenEditor,
                Description = strings.SkinOpenEditorDescription,
                ButtonText = "Open",
                Action = () => game?.OpenSkinEditor(),
                EnabledBindable = buttonsEnabled
            },
            new SettingsButton
            {
                Label = strings.SkinOpenFolder,
                Description = strings.SkinOpenFolderDescription,
                Action = skinManager.OpenFolder,
                ButtonText = "Open"
            },
            new SettingsButton
            {
                Label = strings.SkinExport,
                Description = strings.SkinExportDescription,
                ButtonText = "Export",
                EnabledBindable = buttonsEnabled,
                Action = skinManager.ExportCurrent
            },
            new SettingsButton
            {
                Label = strings.SkinDelete,
                Description = strings.SkinDeleteDescription,
                ButtonText = "Delete",
                EnabledBindable = buttonsEnabled,
                Action = () =>
                {
                    if (skinManager.IsDefault)
                        return;

                    panels.Content = new ConfirmDeletionPanel(() => skinManager.Delete(skinManager.SkinFolder), itemName: "skin");
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        buttonsEnabled.Value = !skinManager.IsDefault;

        skinManager.SkinChanged += () => buttonsEnabled.Value = !skinManager.IsDefault;
        skinManager.SkinListChanged += reloadList;
    }

    private void reloadList() => currentDropdown.Items = skinManager.GetSkinNames();
}
