using System;
using fluXis.Database.Maps;
using fluXis.Integration;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Screens.Gameplay;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Online.Activity;

[JsonObject(MemberSerialization.OptIn)]
public abstract class UserActivity
{
    public IAPIClient API { get; set; }

    public virtual DiscordRichPresence CreateDiscord()
    {
        var rpc = new DiscordRichPresence { LargeImage = "menu" };

        if (API is { IsLoggedIn: true })
        {
            var user = API.User.Value;
            rpc.SmallImage = $"{API.Endpoint.AssetUrl}/avatar/{user.AvatarHash}";
            rpc.SmallImageText = user.Username;
        }

        return rpc;
    }

    public virtual void CreateSteam(ISteamManager steam) { }

    #region Menus

    public class MenuGeneral : UserActivity
    {
        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "In the menus";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "In the menus");
        }
    }

    public class SongSelect : UserActivity
    {
        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Selecting a song";
            rpc.LargeImage = "songselect";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Selecting a song");
        }
    }

    public class Results : UserActivity
    {
        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Viewing results";
            rpc.LargeImage = "results";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Viewing results");
        }
    }

    public class BrowsingMaps : UserActivity
    {
        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Browsing online maps";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Browsing for new maps");
        }
    }

    #endregion

    #region Gameplay/Replays

    public class LoadingGameplay : UserActivity
    {
        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Loading...";
            rpc.LargeImage = "playing";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Loading...");
        }
    }

    public class Playing : UserActivity
    {
        private GameplayScreen screen { get; }
        private RealmMap map { get; }

        public MultiplayerRoom Room { get; set; }

        [JsonProperty("id")]
        public long MapID => map.OnlineID;

        public Playing(GameplayScreen screen, RealmMap map)
        {
            this.screen = screen;
            this.map = map;
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = $"{map.Metadata.Title} - {map.Metadata.Artist} [{map.Difficulty}]";
            rpc.LargeImage = "playing";

            if (!screen.IsPaused.Value)
            {
                var current = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var duration = screen.Map.EndTime - screen.GameplayClock.CurrentTime;
                duration /= screen.Rate;
                rpc.EndTime = current + Convert.ToUInt64(Math.Max(duration, 0));
            }

            if (API is not null && map.MapSet.OnlineID > 0)
                rpc.LargeImage = $"{API.Endpoint.AssetUrl}/cover/{map.MapSet.OnlineID}";

            if (Room is not null)
            {
                rpc.State = Room.Name;
                rpc.PartyID = Room.RoomID;
                rpc.PartySize = Room.Participants.Count;
                rpc.PartyMax = Math.Max(8, Room.Participants.Count * 2);
            }

            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, $"Playing {map.Metadata.SortingTitle} - {map.Metadata.SortingArtist} [{map.Difficulty}]");
        }
    }

    public class Paused : Playing
    {
        public Paused(GameplayScreen screen, RealmMap map)
            : base(screen, map)
        {
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.State = "Paused";
            rpc.EndTime = 0;
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            base.CreateSteam(steam);
            steam.SetRichPresence(SteamRichPresenceKey.Details, "Paused");
        }
    }

    public class WatchingReplay : Playing
    {
        [CanBeNull]
        private APIUser user { get; }

        public WatchingReplay(GameplayScreen screen, RealmMap map, APIUser user = null)
            : base(screen, map)
        {
            this.user = user;
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.State = user != null ? $"Watching {user.Username}'s replay" : "Watching a replay";
            return rpc;
        }
    }

    #endregion

    #region Editing

    public class Editing : UserActivity
    {
        [JsonProperty("time")]
        public long OpenTime { get; set; }

        public Editing(long openTime)
        {
            OpenTime = openTime;
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Editing a map";
            rpc.LargeImage = "editor";
            rpc.StartTime = Convert.ToUInt64(OpenTime);
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Editing");
        }
    }

    public class Modding : Editing
    {
        public Modding(long openTime)
            : base(openTime)
        {
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "Modding a map";
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "Modding a map");
        }
    }

    #endregion

    #region Multiplayer

    public class MultiLobby : UserActivity
    {
        private MultiplayerRoom room { get; }

        [JsonProperty("id")]
        public long RoomID => room.RoomID;

        public MultiLobby(MultiplayerRoom room)
        {
            this.room = room;
        }

        public override DiscordRichPresence CreateDiscord()
        {
            var rpc = base.CreateDiscord();
            rpc.Details = "In a multiplayer lobby";
            rpc.State = room.Name;
            rpc.PartyID = room.RoomID;
            rpc.PartySize = room.Participants.Count;
            rpc.PartyMax = Math.Max(8, room.Participants.Count * 2);
            return rpc;
        }

        public override void CreateSteam(ISteamManager steam)
        {
            steam.SetRichPresence(SteamRichPresenceKey.Status, "In a multiplayer lobby");
        }
    }

    #endregion
}
