using System.Net.Http;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetDeleteRequest : APIRequest<object>
{
    protected override string Path => $"/mapset/{id}";
    protected override HttpMethod Method => HttpMethod.Delete;

    private long id { get; }

    public MapSetDeleteRequest(long id)
    {
        this.id = id;
    }
}
