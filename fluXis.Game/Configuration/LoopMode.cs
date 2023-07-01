using System.ComponentModel;
using JetBrains.Annotations;

namespace fluXis.Game.Configuration;

public enum LoopMode
{
    [UsedImplicitly]
    [Description("Until End of Track")]
    UntilEnd,

    [Description("Limited (only 15 seconds)")]
    Limited
}
