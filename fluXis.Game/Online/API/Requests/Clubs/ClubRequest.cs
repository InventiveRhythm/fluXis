using fluXis.Game.Online.API.Models.Clubs;

namespace fluXis.Game.Online.API.Requests.Clubs;

public class ClubRequest : APIRequest<APIClub>
{
    protected override string Path => $"/club/{id}";

    private long id { get; }

    public ClubRequest(long id)
    {
        this.id = id;
    }
}
