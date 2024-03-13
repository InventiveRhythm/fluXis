using fluXis.Shared.Components.Maps;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapLookupRequest : APIRequest<APIMapLookup>
{
    protected override string Path => "/maps/lookup";

    private string hash { get; }

    public MapLookupRequest(string hash)
    {
        this.hash = hash;
    }

    protected override void CreatePostData(FluXisJsonWebRequest<APIMapLookup> request)
    {
        request.AddParameter("hash", hash);
    }
}
