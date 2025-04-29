#if VELOPACK_BUILD
using System.ComponentModel;

namespace fluXis.Configuration;

public enum ReleaseChannel
{
    [Description("Stable")]
    Stable,

    [Description("Beta")]
    Beta,
}
#endif
