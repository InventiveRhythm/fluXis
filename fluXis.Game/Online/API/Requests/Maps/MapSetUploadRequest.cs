using System.Net.Http;
using fluXis.Game.Database.Maps;
using fluXis.Game.Online.API.Models.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapSetUploadRequest : APIRequest<APIMapSet>
{
    protected override string Path => map.OnlineID != -1 ? $"/map/{map.OnlineID}/update" : "/maps";
    protected override HttpMethod Method => HttpMethod.Post;

    private byte[] file { get; }
    private RealmMapSet map { get; }

    public MapSetUploadRequest(byte[] file, RealmMapSet map)
    {
        this.file = file;
        this.map = map;
    }

    protected override void CreatePostData(JsonWebRequest<APIResponse<APIMapSet>> request)
    {
        request.AddFile("file", file);
    }
}
