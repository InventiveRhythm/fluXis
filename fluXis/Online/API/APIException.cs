using System;
using System.Net;

namespace fluXis.Online.API;

public class APIException : Exception
{
    public HttpStatusCode Status { get; }

    public APIException(string message, HttpStatusCode code)
        : base(message)
    {
        Status = code;
    }
}
