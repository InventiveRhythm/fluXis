using System.Linq;
using fluXis.Database;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins.Capabilities;
using osu.Framework.Bindables;

namespace fluXis.Overlay.Settings.Sections.Plugins.Import;

public partial class DrawableImportPlugin : DrawablePlugin
{
    protected override void AfterLoad()
    {
        // we have to make sure that the plugin has map importing capabilities before proceeding past this point
        if (!Plugin.HasCapability<IMapImporterCapability>()) return;
        if (Plugin.GetCapability<IMapImporterCapability>().Importer == null) return;

        var importer = ImportManager.GetImporter(Plugin);

        if (!importer.SupportsAutoImport) return;

        var autoImport = new Bindable<bool>
        {
            Value = Realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false)
        };

        Flow.Add(new SettingsToggle
        {
            Label = "Auto Import Maps",
            Bindable = autoImport
        });

        autoImport.BindValueChanged(e =>
        {
            Realm.RunWrite(r =>
            {
                var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID);

                if (info is not null)
                    info.AutoImport = e.NewValue;
            });

            if (e.NewValue)
                ImportManager.ImportMapsFrom(ImportManager.GetImporter(Plugin));
            else
                ImportManager.RemoveImportedMaps(ImportManager.GetImporter(Plugin));
        });
    }
}
