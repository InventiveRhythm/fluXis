using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class BeatPulseContainer : CompositeDrawable
{
    private IBeatSyncProvider beatSync { get; set; }

    private List<BeatPulseEvent> events { get; }
    private Drawable target { get; }

    public BeatPulseContainer(List<BeatPulseEvent> events, Drawable target)
    {
        this.events = events;
        this.target = target;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // yes this is stupid,
        // but for some reason its GlobalClock instead of GameplayClock in the load thread
        beatSync = Dependencies.Get<IBeatSyncProvider>();
        beatSync.OnBeat += onBeat;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        beatSync.OnBeat -= onBeat;
    }

    private void onBeat(int idx)
    {
        var time = Time.Current;

        var e = events.LastOrDefault(x => x.Time <= time + 10); // 10ms buffer to prevent missing beats

        if (e == null)
            return;

        var inDuration = beatSync.BeatTime * e.ZoomIn;
        var outDuration = beatSync.BeatTime * (1 - e.ZoomIn);

        target.ScaleTo(e.Strength, inDuration, Easing.OutQuint)
              .Then().ScaleTo(1, outDuration, Easing.OutQuint);
    }
}
