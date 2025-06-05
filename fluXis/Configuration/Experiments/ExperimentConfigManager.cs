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
        SetDefault(ExperimentConfig.StoryboardTab, false);
        SetDefault(ExperimentConfig.ModView, false);
    }
}

public enum ExperimentConfig
{
    StoryboardTab,
    ModView
}
