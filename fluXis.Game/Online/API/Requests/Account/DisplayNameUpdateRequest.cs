using System.Net.Http;

namespace fluXis.Game.Online.API.Requests.Account;

public class DisplayNameUpdateRequest : APIRequest<dynamic>
{
    protected override string Path => "/account/update/displayname";
    protected override HttpMethod Method => HttpMethod.Post;

    private string displayName { get; }

    public DisplayNameUpdateRequest(string displayName)
    {
        this.displayName = displayName;
    }

    protected override void CreatePostData(FluXisJsonWebRequest<dynamic> request)
    {
        request.AddRaw(displayName);
    }
}
