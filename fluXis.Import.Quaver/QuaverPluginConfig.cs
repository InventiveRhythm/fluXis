using System.IO;
using System.Linq;
using fluXis.Game.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.Quaver;

public class QuaverPluginConfig : PluginConfigManager<QuaverPluginSetting>
{
    protected override string ID => "quaver";

    public QuaverPluginConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(QuaverPluginSetting.GameLocation, getLocation());
    }

    private string getLocation()
    {
        const string c_path = @"C:\Program Files (x86)\Steam\steamapps\common\Quaver\Quaver.exe";
        var installPath = "";

        if (File.Exists(c_path)) installPath = c_path;
        else
        {
            string[] drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                              .Select(c => $@"{c}:\")
                              .Where(Directory.Exists)
                              .ToArray();

            const string steam_lib_path = @"SteamLibrary\steamapps\common\Quaver\Quaver.exe";
            const string steam_path = @"Steam\steamapps\common\Quaver\Quaver.exe";

            foreach (var drive in drives)
            {
                if (File.Exists($"{drive}{steam_lib_path}"))
                {
                    installPath = $"{drive}{steam_lib_path}";
                    break;
                }

                if (File.Exists($"{drive}{steam_path}"))
                {
                    installPath = $"{drive}{steam_path}";
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(installPath))
            return "";

        return Path.GetDirectoryName(installPath);
    }
}

public enum QuaverPluginSetting
{
    GameLocation
}
