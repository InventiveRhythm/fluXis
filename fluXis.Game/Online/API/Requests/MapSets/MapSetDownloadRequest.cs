using System.IO;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetDownloadRequest : APIRequest
{
    protected override string Path => $"/mapset/{id}/download";

    public MemoryStream ResponseStream { get; private set; }

    private long id { get; }

    public MapSetDownloadRequest(long id)
    {
        this.id = id;
    }

    protected override void PostProcess()
    {
        ResponseStream = new MemoryStream();
        Request.ResponseStream.CopyTo(ResponseStream);
    }
}
