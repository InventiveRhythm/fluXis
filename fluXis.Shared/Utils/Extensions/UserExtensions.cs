using fluXis.Shared.Components.Users;

namespace fluXis.Shared.Utils.Extensions;

public static class UserExtensions
{
    public static bool IsDeveloper(this APIUserShort user)
        => user.Groups.Any(g => g.ID == "dev");

    public static bool CanModerate(this APIUserShort user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "moderator");
    }
}
