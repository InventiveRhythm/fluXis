using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Integration;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.Sections.Appearance.Skin;
using fluXis.Overlay.Settings.UI;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings.Sections.Appearance;

public partial class AppearanceSkinSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Skin;
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [Resolved]
    private SkinManager skinManager { get; set; }

    private FillFlowContainer skinFlow;
    private BindableBool buttonsEnabled;

    [BackgroundDependencyLoader(true)]
    private void load(SkinManager skinManager, [CanBeNull] FluXisGame game, PanelContainer panels, [CanBeNull] ISteamManager steam)
    {
        buttonsEnabled = new BindableBool(true);

        RightSide.Add(new RefreshButton { Action = skinManager.ReloadSkinList });

        AddRange(new Drawable[]
        {
            skinFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(20),
                ChildrenEnumerable = skinManager.AvailableSkins.Select(x => new SkinIcon(x)),
                Margin = new MarginPadding { Bottom = 8 }
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
                Label = strings.SkinOpenEditor,
                Description = strings.SkinOpenEditorDescription,
                ButtonText = "Open",
                Action = () => game?.OpenSkinEditor(),
                EnabledBindable = buttonsEnabled
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

                    panels.Content = new ConfirmDeletionPanel(() => skinManager.Delete(skinManager.CurrentSkinID), itemName: "skin");
                }
            },
        });

        if (steam?.Initialized ?? false)
        {
            Add(new SettingsButton
            {
                Label = "Upload to Workshop",
                Description = "Publish your skin to the Steam Workshop.",
                ButtonText = "Upload",
                EnabledBindable = buttonsEnabled,
                Action = skinManager.UploadToWorkshop
            });
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        buttonsEnabled.Value = !skinManager.IsDefault;

        skinManager.SkinChanged += () => buttonsEnabled.Value = !skinManager.IsDefault;
        skinManager.SkinListChanged += reloadList;
    }

    private void reloadList()
    {
        skinFlow.Clear();
        skinFlow.ChildrenEnumerable = skinManager.AvailableSkins.Select(x => new SkinIcon(x));
    }

    private partial class RefreshButton : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        private HoverLayer hover { get; }
        private FlashLayer flash { get; }

        public RefreshButton()
        {
            AutoSizeAxes = Axes.X;
            Height = 32;
            CornerRadius = 16;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding(16),
                    Spacing = new Vector2(6),
                    Children = new Drawable[]
                    {
                        new FluXisSpriteIcon
                        {
                            Icon = FontAwesome.Solid.Sync,
                            Size = new Vector2(14),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        },
                        new FluXisSpriteText
                        {
                            Text = "Refresh",
                            WebFontSize = 14,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    }
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();
            samples.Click();
            return base.OnClick(e);
        }
    }
}
