using System;

namespace fluXis.Online.API;

public class APIException : Exception
{
    public APIException(string message)
        : base(message)
    {
    }
}
