using fluXis.Game.Graphics;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;

namespace fluXis.Game.Audio.Preview;

public partial class PreviewManager : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    private ITrackStore trackStore;
    private long currentId = -1;
    private Track track;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        var resources = new ResourceStore<byte[]>();
        resources.AddStore(new OnlineStore());
        resources.AddStore(new HttpOnlineStore());
        trackStore = audio.GetTrackStore(resources);
    }

    public async void PlayPreview(long id)
    {
        if (currentId == id)
            return;

        currentId = id;

        if (track != null)
            await track.StopAsync();

        track = await trackStore.GetAsync($"{api.Endpoint.AssetUrl}/preview/{id}");

        if (track == null)
        {
            Logger.Log($"Failed to load preview track for {id}", LoggingTarget.Runtime, LogLevel.Error);
            return;
        }

        // If the id has changed since we started loading, don't play the track
        if (currentId != id)
            return;

        track.Looping = true;
        await track.RestartAsync();
    }

    public void StopPreview()
    {
        currentId = -1;
        track?.Stop();
    }
}
