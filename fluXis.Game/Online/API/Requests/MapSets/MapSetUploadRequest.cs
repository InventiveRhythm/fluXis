using System.Net.Http;
using fluXis.Shared.Components.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetUploadRequest : APIRequest<APIMapSet>
{
    protected override string Path => update ? $"/mapset/{mapID}" : "/mapsets";
    protected override HttpMethod Method => update ? HttpMethod.Patch : HttpMethod.Post;

    private bool update => mapID != -1;

    private byte[] file { get; }
    private long mapID { get; }

    public MapSetUploadRequest(byte[] file, long mapID)
    {
        this.file = file;
        this.mapID = mapID;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddFile("file", file);
        return req;
    }
}
