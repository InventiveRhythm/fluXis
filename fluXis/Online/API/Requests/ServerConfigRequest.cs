using fluXis.Online.API.Models;

namespace fluXis.Online.API.Requests;

public class ServerConfigRequest : APIRequest<APIConfig>
{
    protected override string Path => "/config";
}
