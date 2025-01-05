using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace fluXis.Configuration.Experiments;

public class ExperimentConfigManager : IniConfigManager<ExperimentConfig>
{
    protected override string Filename => "experiments.ini";

    public ExperimentConfigManager(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
#if CLOSED_TESTING
        SetDefault(ExperimentConfig.DesignTab, true);
        SetDefault(ExperimentConfig.StoryboardTab, true);
#else
        SetDefault(ExperimentConfig.DesignTab, false);
        SetDefault(ExperimentConfig.StoryboardTab, false);
#endif
    }
}

public enum ExperimentConfig
{
    DesignTab,
    StoryboardTab
}
