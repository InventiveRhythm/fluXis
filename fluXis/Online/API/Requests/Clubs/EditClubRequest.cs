using System.Net.Http;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Payloads.Clubs;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Clubs;

public class EditClubRequest : APIRequest<APIClub>
{
    protected override string Path => $"/club/{id}";
    protected override HttpMethod Method => HttpMethod.Patch;

    private long id { get; }
    private EditClubPayload payload { get; }

    public EditClubRequest(long id, EditClubPayload payload)
    {
        this.id = id;
        this.payload = payload;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = payload.Serialize();
        req.AddRaw(json);
        return req;
    }
}
