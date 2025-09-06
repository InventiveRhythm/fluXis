using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database;
using fluXis.Graphics.Containers;
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

namespace fluXis.Tests.Plugins;

public abstract partial class TestPluginSettings : FluXisTestScene
{
    public abstract Type TargetType { get; }

    [BackgroundDependencyLoader]
    private void load(PluginManager plugins, ImportManager imports, FluXisRealm realm)
    {
        var plugin = plugins.Plugins.FirstOrDefault(x => x.GetType() == TargetType) ?? throw new InvalidOperationException("Target type is not a loaded plugin.");
        var importer = plugin.Importer;

        var list = new List<Drawable>();

        if (importer?.SupportsAutoImport ?? false)
        {
            var autoImport = new Bindable<bool>
            {
                Value = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false)
            };

            list.Add(new SettingsToggle
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
                    imports.ImportMapsFrom(imports.GetImporter(plugin));
                else
                    imports.RemoveImportedMaps(imports.GetImporter(plugin));
            });
        }

        list.AddRange(plugin.CreateSettings());

        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(12),
                    Padding = new MarginPadding(48) { Left = 72 },
                    Children = list
                }
            }
        });
    }
}
