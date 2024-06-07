using System.Net.Http;
using fluXis.Game.Database.Maps;
using fluXis.Shared.Components.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetUploadRequest : APIRequest<APIMapSet>
{
    protected override string Path => update ? $"/mapset/{map.OnlineID}" : "/mapsets";
    protected override HttpMethod Method => update ? HttpMethod.Patch : HttpMethod.Post;

    private bool update => map.OnlineID != -1;

    private byte[] file { get; }
    private RealmMapSet map { get; }

    public MapSetUploadRequest(byte[] file, RealmMapSet map)
    {
        this.file = file;
        this.map = map;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddFile("file", file);
        return req;
    }
}
