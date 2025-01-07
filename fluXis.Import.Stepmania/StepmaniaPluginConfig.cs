using fluXis.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.Stepmania;

public class StepmaniaPluginConfig : PluginConfigManager<StepmaniaPluginSetting>
{
    protected override string ID => "stepmania";

    public StepmaniaPluginConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(StepmaniaPluginSetting.GameLocation, "");
    }
}

public enum StepmaniaPluginSetting
{
    GameLocation
}
