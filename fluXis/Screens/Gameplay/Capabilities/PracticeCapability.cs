using System.Linq;
using fluXis.Map;
using fluXis.Screens.Gameplay.Capabilities.Bases;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public class PracticeCapability : IMapCapability
{
    public GameplayScreen Screen { get; set; } = null!;

    private float start { get; }
    private float end { get; }

    public PracticeCapability(float start, float end)
    {
        this.start = start;
        this.end = end;
    }

    void IGameplayCapability.PreLoad()
    {
        Screen.GameplayStartTime = start;
    }

    void IMapCapability.Modify(MapInfo map)
    {
        map.HitObjects = map.HitObjects.Where(o => o.Time >= start && o.Time <= end).ToList();
    }
}
