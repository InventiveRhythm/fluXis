namespace fluXis.Shared.API;

public static class PacketIDs
{
    public const string AUTH = "account/auth";
    public const string LOGIN = "account/login";
    public const string REGISTER = "account/register";
    public const string LOGOUT = "account/logout";

    public const string ACHIEVEMENT = "achievement";
    public const string SERVER_MESSAGE = "server/message";
    public const string MAINTENANCE = "server/maintenance";

    public const string FRIEND_ONLINE = "friend/online";
    public const string FRIEND_OFFLINE = "friend/offline";

    public const string CHAT_MESSAGE = "chat/message";
    public const string CHAT_HISTORY = "chat/history";
    public const string CHAT_DELETE = "chat/delete";

    public const string MULTIPLAYER_CREATE = "multi/create";
    public const string MULTIPLAYER_JOIN = "multi/join";
    public const string MULTIPLAYER_LEAVE = "multi/leave";
    public const string MULTIPLAYER_KICK = "multi/kick";
    public const string MULTIPLAYER_STATE = "multi/state"; // player state update
    public const string MULTIPLAYER_MAP = "multi/map";
    public const string MULTIPLAYER_UPDATE = "multi/update";
    public const string MULTIPLAYER_READY = "multi/ready"; // player ready state
    public const string MULTIPLAYER_START = "multi/start"; // game start
    public const string MULTIPLAYER_COMPLETE = "multi/complete"; // player finished
    public const string MULTIPLAYER_FINISH = "multi/finish"; // game finished
    public const string MULTIPLAYER_SCORE = "multi/score"; // mid-game score update
}
