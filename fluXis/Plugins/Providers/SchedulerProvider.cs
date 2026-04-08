using System;
using fluXis.Plugins.Capabilities;

namespace fluXis.Plugins.Providers;

internal class GameSchedulerProvider : ISchedulerProvider
{
    private readonly FluXisGame game;

    public GameSchedulerProvider(FluXisGame game)
    {
        this.game = game;
    }

    public void Schedule(Action action)
    {
        game.Schedule(action);
    }
}
