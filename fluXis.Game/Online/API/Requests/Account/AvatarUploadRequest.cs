using System.IO;

namespace fluXis.Game.Online.API.Requests.Account;

public class AvatarUploadRequest : ImageUpdateRequest
{
    protected override string Path => "/account/update/avatar";

    public AvatarUploadRequest(FileInfo file)
        : base(file)
    {
    }
}
