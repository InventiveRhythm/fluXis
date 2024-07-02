using fluXis.Game.Database.Maps;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Users;
using JetBrains.Annotations;

namespace fluXis.Game.Online.Activity;

public abstract class UserActivity
{
    public abstract string Status { get; }
    public virtual string Details => "";
    public virtual string Icon => "icon";

    public IAPIClient API { get; set; }

    #region Menus

    public class MenuGeneral : UserActivity
    {
        public override string Status => "In the menus";
        public override string Icon => "menu";
    }

    public class SongSelect : UserActivity
    {
        public override string Status => "Selecting a song";
        public override string Icon => "songselect";
    }

    public class Results : UserActivity
    {
        public override string Status => "Viewing results";
        public override string Icon => "results";
    }

    public class BrowsingMaps : UserActivity
    {
        public override string Status => "Browsing online maps";
        public override string Icon => "menu";
    }

    #endregion

    #region Gameplay/Replays

    public class LoadingGameplay : UserActivity
    {
        public override string Status => "Loading...";
        public override string Icon => "playing";
    }

    public class Playing : UserActivity
    {
        public override string Status => "Playing";
        public override string Details => $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Difficulty}]";
        public override string Icon => Map.MapSet.OnlineID <= 0 ? "playing" : $"{API?.Endpoint.AssetUrl}/cover/{Map.MapSet.OnlineID}";

        protected RealmMap Map { get; init; }

        public Playing(RealmMap map)
        {
            Map = map;
        }
    }

    public class Paused : Playing
    {
        public override string Status => "Paused";

        public Paused(RealmMap map)
            : base(map)
        {
        }
    }

    public class WatchingReplay : Playing
    {
        public override string Status => user != null ? $"Watching {user.Username}'s replay" : "Watching a replay";

        [CanBeNull]
        private APIUser user { get; init; }

        public WatchingReplay(RealmMap map, APIUser user = null)
            : base(map)
        {
            this.user = user;
        }
    }

    #endregion

    #region Editing

    public class Editing : UserActivity
    {
        public override string Status => "Editing a map";
        public override string Icon => "editor";
    }

    public class Modding : Editing
    {
        public override string Status => "Modding a map";
    }

    #endregion
}
