using System;

namespace fluXis.Utils.Exceptions;

public class SteamInitException : Exception
{
    public SteamInitException()
        : base("Failed to initialize steam connection. Please open the game through steam.")
    {
    }
}
