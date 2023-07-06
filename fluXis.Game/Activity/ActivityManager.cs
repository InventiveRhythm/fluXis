using System.Collections.Generic;
using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Activity;

public class ActivityManager
{
    private readonly List<IActivity> activities = new();
    private readonly Fluxel fluxel;

    public ActivityManager(Fluxel fluxel)
    {
        this.fluxel = fluxel;
    }

    public void Add(IActivity activity)
    {
        activities.Add(activity);
        activity.Initialize();
    }

    public void Update(string details = "", string state = "", string largeImageKey = "", int timestamp = 0, int timeLeft = 0)
    {
        foreach (IActivity activity in activities)
            activity.Update(fluxel, details, state, largeImageKey, timestamp, timeLeft);
    }
}
