using Midori.Networking.WebSockets.Typed;

namespace fluXis.Online.Exceptions;

public class RoomException : TypedWebSocketException
{
    protected RoomException(string message)
        : base(message)
    {
    }

    public static RoomException AlreadyInRoom() => new("Already in a room.");
    public static RoomException NotInRoom() => new("Not in a room.");
    public static RoomException RoomNotFound() => new("The room could not be found.");
    public static RoomException NotHost() => new("You are not the host of this room.");
}
