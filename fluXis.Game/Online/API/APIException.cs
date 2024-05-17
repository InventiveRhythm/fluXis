using System;

namespace fluXis.Game.Online.API;

public class APIException : Exception
{
    public APIException(string message)
        : base(message)
    {
    }
}
