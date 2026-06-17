using fluXis.Online.API.Models.Clubs;

namespace fluXis.Online.API.Requests.Invites;

public class GetInviteRequest : APIRequest<APIClubInvite>
{
    protected override string Path => $"/invites/{code}";

    private string code { get; }

    public GetInviteRequest(string code)
    {
        this.code = code;
    }
}
