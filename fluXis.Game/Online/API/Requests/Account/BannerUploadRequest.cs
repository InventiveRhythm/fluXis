using System.IO;

namespace fluXis.Game.Online.API.Requests.Account;

public class BannerUploadRequest : ImageUpdateRequest
{
    protected override string Path => "/account/update/banner";

    public BannerUploadRequest(FileInfo file)
        : base(file)
    {
    }
}
