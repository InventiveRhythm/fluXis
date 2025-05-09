using fluXis.Online.API.Models.Other;

namespace fluXis.Online.API.Requests.Invites;

public class GetInviteRequest : APIRequest<APIInvite>
{
    protected override string Path => $"/invites/{code}";

    private string code { get; }

    public GetInviteRequest(string code)
    {
        this.code = code;
    }
}
