using Midori.Networking.WebSockets.Typed;

namespace fluXis.Online.Exceptions;

public class MultiMapException : TypedWebSocketException
{
    protected MultiMapException(string message)
        : base(message)
    {
    }

    public static MultiMapException NotFound() => new("The requested map could not be found.");
    public static MultiMapException Mismatch() => new("Your local version of the map does not match the one on the server.");
}
