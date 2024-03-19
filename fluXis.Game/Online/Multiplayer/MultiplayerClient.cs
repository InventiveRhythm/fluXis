using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Multiplayer;

public abstract partial class MultiplayerClient : Component, IMultiplayerClient
{
    public event Action<APIUserShort> UserJoined;
    public event Action<APIUserShort> UserLeft;
    public event Action RoomUpdated;

    public event Action<string> MapChangedFailed;
    public event Action<IAPIMapShort> MapChanged;

    public event Action<long, bool> ReadyStateChanged;
    public event Action Starting;
    public event Action<List<ScoreInfo>> ResultsReady;

    public virtual APIUserShort Player => APIUserShort.Dummy;
    public MultiplayerRoom Room { get; set; }

    public async Task Create(string name, long mapid, string hash)
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot create a room while already in one");

        Room = await CreateRoom(name, mapid, hash);
    }

    protected abstract Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash);

    public async Task Join(MultiplayerRoom room, string password = "")
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot join a room while already in one");

        Room = await JoinRoom(room.RoomID, password);
    }

    protected abstract Task<MultiplayerRoom> JoinRoom(long id, string password);

    public virtual Task LeaveRoom()
    {
        Room = null;
        return Task.CompletedTask;
    }

    Task IMultiplayerClient.UserJoined(APIUserShort user)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (Room.Users.Any(u => u.ID == user.ID))
                return;

            Room.Users.Add(user);

            UserJoined?.Invoke(user);
            RoomUpdated?.Invoke();
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.UserLeft(long id) => handleLeave(id, UserLeft);

    Task IMultiplayerClient.SettingsChanged(MultiplayerRoom room) => Task.CompletedTask;

    public virtual Task ChangeMap(long map, string hash)
    {
        throw new NotImplementedException();
    }

    Task IMultiplayerClient.MapChanged(bool success, IAPIMapShort map, string error)
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

    Task IMultiplayerClient.ReadyStateChanged(long userId, bool isReady)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            ReadyStateChanged?.Invoke(userId, isReady);
            RoomUpdated?.Invoke();
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.Starting()
    {
        Schedule(() => Starting?.Invoke());
        return Task.CompletedTask;
    }

    private Task handleLeave(long id, Action<APIUserShort> callback)
    {
        Scheduler.Add(() =>
        {
            if (Room?.Users.FirstOrDefault(u => u.ID == id) is not { } user)
                return;

            Room.Users.Remove(user);

            callback?.Invoke(user);
            RoomUpdated?.Invoke();
        }, false);

        return Task.CompletedTask;
    }

    public virtual Task Finished(ScoreInfo score) => Task.CompletedTask;

    Task IMultiplayerClient.ResultsReady(List<ScoreInfo> scores)
    {
        Logger.Log($"Received results for {scores.Count} players", LoggingTarget.Network, LogLevel.Debug);
        Schedule(() => ResultsReady?.Invoke(scores));
        return Task.CompletedTask;
    }
}
