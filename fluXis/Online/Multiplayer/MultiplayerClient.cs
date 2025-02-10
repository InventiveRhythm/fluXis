using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;

namespace fluXis.Online.Multiplayer;

public abstract partial class MultiplayerClient : Component, IMultiplayerClient
{
    public event Action<MultiplayerParticipant> OnUserJoin;
    public event Action<MultiplayerParticipant> OnUserLeave;
    public event Action<long, MultiplayerUserState> OnUserStateChange;
    public event Action<long> OnHostChange;

    // public event Action RoomUpdated;

    public event Action<APIMap> OnMapChange;

    public event Action OnStart;
    public event Action<long, int> OnScore;
    public event Action<List<ScoreInfo>> OnResultsReady;

    public event Action OnDisconnect;

    public virtual APIUser Player => APIUser.Dummy;
    public MultiplayerRoom Room { get; set; }

    public async Task Create(string name, long mapid, string hash)
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot create a room while already in one");

        Room = await CreateRoom(name, mapid, hash);
    }

    public async Task Join(long id, string password = "")
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot join a room while already in one");

        Room = await JoinRoom(id, password);
    }

    Task IMultiplayerClient.UserJoined(MultiplayerParticipant participant)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (Room.Participants.Any(u => u.ID == participant.ID))
                return;

            Room.Participants.Add(participant);
            OnUserJoin?.Invoke(participant);
        });

        return Task.CompletedTask;
    }

    async Task IMultiplayerClient.UserLeft(long id)
    {
        await handleLeave(id, OnUserLeave);
    }

    public Task UserStateChanged(long id, MultiplayerUserState state)
    {
        Schedule(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            participant.State = state;
            OnUserStateChange?.Invoke(id, state);
        });

        return Task.CompletedTask;
    }

    public Task HostChanged(long id)
    {
        if (Room is null)
            return Task.CompletedTask;

        Schedule(() =>
        {
            var part = Room.Participants.FirstOrDefault(x => x.ID == id);
            Room.Host = part?.User ?? APIUser.CreateUnknown(id);
            OnHostChange?.Invoke(id);
        });

        return Task.CompletedTask;
    }

    public Task MapUpdated(APIMap map)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            Room.Map = map;
            OnMapChange?.Invoke(map);
        });

        return Task.CompletedTask;
    }

    public Task LoadRequested()
    {
        Schedule(() =>
        {
            Room.Scores = new List<ScoreInfo>();
            Room.Scores.AddRange(Room.Participants.Where(p => p.ID != Player.ID).Select(p => new ScoreInfo { PlayerID = p.ID }));

            OnStart?.Invoke();
        });

        return Task.CompletedTask;
    }

    public Task ScoreUpdated(long user, int score)
    {
        if (user == Player.ID)
            return Task.CompletedTask;

        OnScore?.Invoke(user, score);

        return Task.CompletedTask;
    }

    public Task EveryoneFinished(List<ScoreInfo> scores)
    {
        // dangerous but when this gets called the
        // client is unable to execute schedules
        // might need a better fix someday
        OnResultsReady?.Invoke(scores);
        return Task.CompletedTask;
    }

    private Task handleLeave(long id, Action<MultiplayerParticipant> callback)
    {
        Scheduler.Add(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            Room.Participants.Remove(participant);

            callback?.Invoke(participant);
        }, false);

        return Task.CompletedTask;
    }

    protected void TriggerDisconnect()
    {
        Room = null;
        Scheduler.ScheduleIfNeeded(() => OnDisconnect?.Invoke());
    }

    #region Abstract Methods

    protected abstract Task<MultiplayerRoom> JoinRoom(long id, string password);
    protected abstract Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash);
    public abstract Task LeaveRoom();
    public abstract Task ChangeMap(long map, string hash);
    public abstract Task TransferHost(long target);
    public abstract Task UpdateScore(int score);
    public abstract Task Finish(ScoreInfo score);
    public abstract Task SetReadyState(bool ready);

    #endregion
}
