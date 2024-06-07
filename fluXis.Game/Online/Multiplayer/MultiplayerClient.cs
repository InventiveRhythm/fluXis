using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Multi;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Multiplayer;

public abstract partial class MultiplayerClient : Component, IMultiplayerClient
{
    public event Action<MultiplayerParticipant> UserJoined;
    public event Action<MultiplayerParticipant> UserLeft;
    public event Action<long, MultiplayerUserState> UserStateChanged;

    // public event Action RoomUpdated;

    public event Action<string> MapChangedFailed;
    public event Action<APIMap> MapChanged;

    public event Action Starting;
    public event Action<List<ScoreInfo>> ResultsReady;

    public virtual APIUser Player => APIUser.Dummy;
    public MultiplayerRoom Room { get; set; }

    public async Task Create(string name, long mapid, string hash)
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot create a room while already in one");

        Room = await CreateRoom(name, mapid, hash);
    }

    public async Task Join(MultiplayerRoom room, string password = "")
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot join a room while already in one");

        Room = await JoinRoom(room.RoomID, password);
    }

    #region Abstract Methods

    protected abstract Task<MultiplayerRoom> JoinRoom(long id, string password);
    protected abstract Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash);
    public abstract Task LeaveRoom();
    public abstract Task ChangeMap(long map, string hash);
    public abstract Task Finish(ScoreInfo score);

    #endregion

    #region IMultiplayerClient Implementation

    Task IMultiplayerClient.UserJoined(MultiplayerParticipant participant)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (Room.Participants.Any(u => u.ID == participant.ID))
                return;

            Room.Participants.Add(participant);

            UserJoined?.Invoke(participant);
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.UserLeft(long id) => handleLeave(id, UserLeft);

    Task IMultiplayerClient.UserStateChanged(long id, MultiplayerUserState state)
    {
        Schedule(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            var user = participant as MultiplayerParticipant;
            user!.State = state;

            UserStateChanged?.Invoke(id, state);
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.SettingsChanged(MultiplayerRoom room) => Task.CompletedTask;

    Task IMultiplayerClient.MapChanged(bool success, APIMap map, string error)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (!success)
            {
                MapChangedFailed?.Invoke(error);
                MapChanged?.Invoke(Room.Map);
                return;
            }

            Room.Map = map;
            MapChanged?.Invoke(map);
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.Starting()
    {
        Schedule(() => Starting?.Invoke());
        return Task.CompletedTask;
    }

    private Task handleLeave(long id, Action<MultiplayerParticipant> callback)
    {
        Scheduler.Add(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            Room.Participants.Remove(participant);

            callback?.Invoke(participant as MultiplayerParticipant);
        }, false);

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.ResultsReady(List<ScoreInfo> scores)
    {
        Logger.Log($"Received results for {scores.Count} players", LoggingTarget.Network, LogLevel.Debug);
        Schedule(() => ResultsReady?.Invoke(scores));
        return Task.CompletedTask;
    }

    #endregion
}
