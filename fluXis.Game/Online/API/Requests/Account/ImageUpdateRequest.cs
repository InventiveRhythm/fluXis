using System.IO;
using System.Net.Http;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Account;

public abstract class ImageUpdateRequest : APIRequest<object>
{
    protected override HttpMethod Method => HttpMethod.Post;

    private readonly FileInfo file;

    protected ImageUpdateRequest(FileInfo file)
    {
        this.file = file;
    }

    protected override void CreatePostData(JsonWebRequest<APIResponse<object>> request)
    {
        var bytes = File.ReadAllBytes(file.FullName);
        request.AddFile("file", bytes);
    }
}
