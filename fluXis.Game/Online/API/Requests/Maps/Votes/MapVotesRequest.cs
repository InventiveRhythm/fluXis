using fluXis.Shared.Components.Maps;

namespace fluXis.Game.Online.API.Requests.Maps.Votes;

public class MapVotesRequest : APIRequest<APIMapVotes>
{
    protected override string Path => $"/map/{id}/votes";

    private long id { get; }

    public MapVotesRequest(long id)
    {
        this.id = id;
    }
}
