using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.MapSet.Buttons;

public partial class MapSetDownloadButton : MapSetButton
{
    [Resolved]
    private MapStore maps { get; set; }

    private bool downloaded => maps.MapSets.Any(s => s.OnlineID == set.ID);

    private APIMapSet set { get; }
    private CircularProgress progress;

    public MapSetDownloadButton(APIMapSet set)
        : base(FontAwesome6.Solid.ArrowDown, () => { })
    {
        this.set = set;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        ScaleContainer.Add(progress = new CircularProgress
        {
            RelativeSizeAxes = Axes.Both,
            RoundedCaps = true,
            InnerRadius = .2f,
            Alpha = 0
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.DownloadStarted += downloadStart;
        maps.DownloadFinished += downloadFinish;
        maps.MapSetAdded += setAdded;

        updateIcon();
        FinishTransforms(true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.DownloadStarted -= downloadStart;
        maps.DownloadFinished -= downloadFinish;
        maps.MapSetAdded -= setAdded;
    }

    private void setAdded(RealmMapSet _) => updateIcon();

    private void downloadStart(MapStore.DownloadStatus status)
    {
        if (status.OnlineID != set.ID)
            return;

        status.StateChanged += stateUpdate;
        status.OnProgress += progressUpdate;

        stateUpdate(status.State);
        progressUpdate(status.Progress);
    }

    private void downloadFinish(MapStore.DownloadStatus status)
    {
        status.StateChanged -= stateUpdate;
        status.OnProgress -= progressUpdate;
        stateUpdate(status.State);
    }

    private void progressUpdate(float prog)
        => Scheduler.ScheduleIfNeeded(() => progress.ProgressTo(prog, 100, Easing.OutQuint));

    private void stateUpdate(MapStore.DownloadState state) => Scheduler.ScheduleIfNeeded(() =>
    {
        switch (state)
        {
            case MapStore.DownloadState.Downloading:
                progress.FadeIn(300);
                break;

            case MapStore.DownloadState.Importing:
                progress.ProgressTo(0, 300, Easing.OutQuint);
                break;

            default:
                progress.ProgressTo(1, 300, Easing.OutQuint);
                progress.FadeOut(300);
                break;
        }

        updateIcon();
    });

    private void updateIcon() => Scheduler.ScheduleIfNeeded(() =>
    {
        Icon.Icon = downloaded ? FontAwesome6.Solid.AngleDoubleRight : FontAwesome6.Solid.ArrowDown;

        var status = maps.DownloadQueue.FirstOrDefault(s => s.OnlineID == set.ID);
        var state = status?.State ?? (downloaded ? MapStore.DownloadState.Finished : null);

        var color = Colour4.White;

        switch (state)
        {
            case MapStore.DownloadState.Importing:
                color = FluXisColors.Highlight.Lighten(.8f);
                break;

            case MapStore.DownloadState.Finished:
                color = FluXisColors.Green.Lighten(1.2f);
                break;

            case MapStore.DownloadState.Failed:
                color = FluXisColors.Red.Lighten(1.2f);
                break;
        }

        this.FadeColour(color, 300);
    });

    protected override bool OnClick(ClickEvent e)
    {
        if (downloaded)
        {
            maps.Present(maps.MapSets.FirstOrDefault(s => s.OnlineID == set.ID));
            return base.OnClick(e);
        }

        maps.DownloadMapSet(set);
        return base.OnClick(e);
    }
}
