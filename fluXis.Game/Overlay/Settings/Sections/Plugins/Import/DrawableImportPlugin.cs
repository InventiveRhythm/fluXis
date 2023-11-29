using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Settings.Sections.Plugins.Import;

public partial class DrawableImportPlugin : Container
{
    public ImportPlugin Plugin { get; init; }

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm, ImportManager importManager)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;

        FillFlowContainer flow;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Children = new[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = Plugin.Name,
                                        FontSize = 24
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"by {Plugin.Author}",
                                        Colour = FluXisColors.Text2,
                                        FontSize = 16
                                    }
                                }
                            },
                            new FluXisSpriteText
                            {
                                Text = Plugin.Version.ToString(),
                                FontSize = 16,
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
                            }
                        }
                    }
                }
            }
        };

        if (Plugin.HasAutoImport)
        {
            var autoImport = new Bindable<bool>
            {
                Value = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == Plugin.ImporterId)?.AutoImport ?? false)
            };

            flow.Add(new SettingsDivider());
            flow.Add(new SettingsToggle
            {
                Label = "Auto Import Maps",
                Bindable = autoImport
            });

            autoImport.BindValueChanged(e =>
            {
                realm.RunWrite(r =>
                {
                    var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == Plugin.ImporterId);

                    if (info is not null)
                        info.AutoImport = e.NewValue;
                });

                if (e.NewValue)
                    importManager.ImportMapsFrom(importManager.GetImporter(Plugin));
                else
                    importManager.RemoveImportedMaps(importManager.GetImporter(Plugin));
            });
        }
    }
}
