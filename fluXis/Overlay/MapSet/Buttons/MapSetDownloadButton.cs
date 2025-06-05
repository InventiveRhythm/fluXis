using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Online.API.Models.Maps;
using fluXis.Utils.Downloading;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace fluXis.Overlay.MapSet.Buttons;

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

    private void downloadStart(DownloadStatus status)
    {
        if (status.OnlineID != set.ID)
            return;

        status.StateChanged += stateUpdate;
        status.OnProgress += progressUpdate;

        stateUpdate(status.State);
        progressUpdate(status.Progress);
    }

    private void downloadFinish(DownloadStatus status)
    {
        status.StateChanged -= stateUpdate;
        status.OnProgress -= progressUpdate;
        stateUpdate(status.State);
    }

    private void progressUpdate(float prog)
        => Scheduler.ScheduleIfNeeded(() => progress.ProgressTo(prog, 100, Easing.OutQuint));

    private void stateUpdate(DownloadState state) => Scheduler.ScheduleIfNeeded(() =>
    {
        switch (state)
        {
            case DownloadState.Downloading:
                progress.FadeIn(300);
                break;

            case DownloadState.Importing:
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
        var state = status?.State ?? (downloaded ? DownloadState.Finished : null);

        var color = Colour4.White;

        switch (state)
        {
            case DownloadState.Importing:
                color = FluXisColors.Highlight.Lighten(.8f);
                break;

            case DownloadState.Finished:
                color = FluXisColors.Green.Lighten(1.2f);
                break;

            case DownloadState.Failed:
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
