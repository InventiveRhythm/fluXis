using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace fluXis.Import.Quaver.Map.Structs;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class QuaverTimingGroup
{
    public List<QuaverSliderVelocityInfo> ScrollVelocities { get; set; }
    public List<QuaverScrollFactor> ScrollSpeedFactors { get; set; }
}
