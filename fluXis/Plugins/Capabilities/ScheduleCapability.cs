using System;

namespace fluXis.Plugins.Capabilities;

public interface ISchedulerProvider
{
    void Schedule(Action action);
}

public static class SchedulerCapability
{
    public static void Schedule(Action action)
        => CapabilityRegistry.Get<ISchedulerProvider>().Schedule(action);
}
