using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Multiplayer;
using fluXis.Scoring;

namespace fluXis.Tests;

public partial class TestMultiplayerClient : MultiplayerClient
{
    public override bool Connected => true;

    public void AddPlayer(long id, string name) => AddPlayer(new APIUser { ID = id, Username = name });

    public void AddPlayer(APIUser user) => Room?.Participants.Add(new MultiplayerParticipant
    {
        Player = user
    });

    protected override Task<MultiplayerRoom> JoinRoom(long id, string password) => Task.FromResult(Room = new MultiplayerRoom
    {
        RoomID = id,
        Host = Player,
        Map = new APIMap(),
        Name = "Test Room",
        Participants = new List<MultiplayerParticipant> { new() { Player = Player } }
    });

    protected override async Task<MultiplayerRoom> CreateRoom(string name, MultiplayerPrivacy privacy, string password, long mapid, string hash)
    {
        throw new System.NotImplementedException();
    }

    public override async Task LeaveRoom()
    {
        throw new System.NotImplementedException();
    }

    public override async Task ChangeMap(long map, string hash, List<string> mods)
    {
        throw new System.NotImplementedException();
    }

    public override async Task TransferHost(long target)
    {
        throw new System.NotImplementedException();
    }

    public override async Task UpdateScore(int score)
    {
        throw new System.NotImplementedException();
    }

    public override async Task Finish(ScoreInfo score)
    {
        throw new System.NotImplementedException();
    }

    public override async Task SetReadyState(bool ready)
    {
        throw new System.NotImplementedException();
    }
}
