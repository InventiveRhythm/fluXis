using System.Net.Http;

namespace fluXis.Game.Online.API.Requests.Account;

public class AboutMeUpdateRequest : APIRequest<dynamic>
{
    protected override string Path => "/account/update/aboutme";
    protected override HttpMethod Method => HttpMethod.Post;

    private string about { get; }

    public AboutMeUpdateRequest(string about)
    {
        this.about = about;
    }

    protected override void CreatePostData(FluXisJsonWebRequest<dynamic> request)
    {
        request.AddRaw(about);
    }
}
