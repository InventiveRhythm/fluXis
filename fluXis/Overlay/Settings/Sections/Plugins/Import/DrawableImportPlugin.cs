using System.Linq;
using fluXis.Database;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Import;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Settings.Sections.Plugins.Import;

public partial class DrawableImportPlugin : Container
{
    public Plugin Plugin { get; init; }

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
                Spacing = new Vector2(10),
                Direction = FillDirection.Vertical,
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

        // we are sure that the plugin has an importer because we only add plugins with importers
        var importer = importManager.GetImporter(Plugin)!;

        if (importer.SupportsAutoImport)
        {
            var autoImport = new Bindable<bool>
            {
                Value = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false)
            };

            flow.Add(new SettingsToggle
            {
                Label = "Auto Import Maps",
                Bindable = autoImport
            });

            autoImport.BindValueChanged(e =>
            {
                realm.RunWrite(r =>
                {
                    var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID);

                    if (info is not null)
                        info.AutoImport = e.NewValue;
                });

                if (e.NewValue)
                    importManager.ImportMapsFrom(importManager.GetImporter(Plugin));
                else
                    importManager.RemoveImportedMaps(importManager.GetImporter(Plugin));
            });
        }

        flow.AddRange(Plugin.CreateSettings());
    }
}
