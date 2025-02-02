using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;

namespace fluXis.Screens.Gameplay.HUD;

public interface IHUDDependencyProvider
{
    JudgementProcessor JudgementProcessor { get; }
    HealthProcessor HealthProcessor { get; }
    ScoreProcessor ScoreProcessor { get; }
    HitWindows HitWindows { get; }

    RealmMap RealmMap { get; }
    MapInfo MapInfo { get; }

    float PlaybackRate { get; }
    double CurrentTime { get; }
}
