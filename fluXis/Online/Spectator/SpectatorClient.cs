using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Mods;
using fluXis.Online.Spectator.Models;
using fluXis.Replays;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;

namespace fluXis.Online.Spectator;

public abstract partial class SpectatorClient : Component, ISpectatorClient
{
    public event Action OnDisconnect;

    protected virtual void TriggerDisconnect()
    {
        frames.Clear();
        Replays.Clear();
        Scheduler.ScheduleIfNeeded(() => OnDisconnect?.Invoke());
    }

    #region Player

    private readonly List<ReplayFrame> frames = new();
    private double nextSyncTime;

    public void BufferFrame(ReplayFrame frame)
    {
        if (frame.Time > nextSyncTime)
            sendBuffer();

        frames.Add(frame);
    }

    public void UpdateTime(double time)
    {
        if (time > nextSyncTime)
            sendBuffer();
    }

    private void sendBuffer()
    {
        frames.Add(new ReplayFrame(nextSyncTime));
        var bundle = new SpectatorFrameBundle(frames.ToList());
        frames.Clear();
        nextSyncTime += ReplayFrame.SYNC_INTERVAL;
        SendBundle(bundle);
    }

    public void StartPlaying(long mapid, IEnumerable<IMod> mods)
    {
        frames.Clear();
        nextSyncTime = 0;
        StartPlayingCore(mapid, mods.Select(x => x.Acronym));
    }

    public abstract void StopPlaying();

    protected abstract void StartPlayingCore(long mapid, IEnumerable<string> mods);
    protected abstract void SendBundle(SpectatorFrameBundle bundle);

    #endregion

    #region Viewer

    public event Action<long, SpectatorState> OnStartedPlaying;
    public event Action<long> OnStoppedPlaying;
    public Dictionary<long, Replay> Replays { get; } = new();

    public abstract Task StartWatching(long id);
    public abstract Task StopWatching(long id);

    Task ISpectatorClient.StartedPlaying(long id, SpectatorState state)
    {
        Schedule(() =>
        {
            Replays.Remove(id);
            Replays.Add(id, new Replay { PlayerID = id });
            OnStartedPlaying?.Invoke(id, state);
        });
        return Task.CompletedTask;
    }

    Task ISpectatorClient.ReceiveFrameBundle(long id, SpectatorFrameBundle bundle)
    {
        if (Replays.TryGetValue(id, out var replay))
        {
            var maxSync = bundle.Frames.Where(x => x.Type == ReplayFrameType.Sync).MaxBy(x => x.Time);
            replay.LastSync = maxSync.Time;
            replay.Frames.AddRange(bundle.Frames);
        }

        return Task.CompletedTask;
    }

    Task ISpectatorClient.StoppedPlaying(long id)
    {
        Schedule(() =>
        {
            OnStoppedPlaying?.Invoke(id);
            Replays.Remove(id);
        });

        return Task.CompletedTask;
    }

    #endregion
}
