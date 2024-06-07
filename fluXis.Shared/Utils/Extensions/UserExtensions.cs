using fluXis.Shared.Components.Users;

namespace fluXis.Shared.Utils.Extensions;

public static class UserExtensions
{
    public static bool IsDeveloper(this APIUser user)
        => user.Groups.Any(g => g.ID == "dev");

    public static bool IsPurifier(this APIUser user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "purifier");
    }

    public static bool CanModerate(this APIUser user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "moderator");
    }
}
