using fluXis.Online.API.Models.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Maps;

public class MapLookupRequest : APIRequest<APIMapLookup>
{
    protected override string Path => "/maps/lookup";

    public string Hash { get; init; }
    public long MapperID { get; init; }
    public string Title { get; init; }
    public string Artist { get; init; }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);

        if (!string.IsNullOrEmpty(Hash))
            req.AddParameter("hash", Hash);
        if (MapperID != 0)
            req.AddParameter("mapper", $"{MapperID}");
        if (!string.IsNullOrEmpty(Title))
            req.AddParameter("title", Title);
        if (!string.IsNullOrEmpty(Artist))
            req.AddParameter("artist", Artist);

        return req;
    }
}
