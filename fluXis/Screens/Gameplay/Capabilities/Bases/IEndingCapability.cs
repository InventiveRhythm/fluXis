using System;
using fluXis.Scoring;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities.Bases;

#nullable enable

public interface IEndingCapability : IGameplayCapability
{
    Screen? OnEnd(ScoreInfo score, Action complete);
}
