using fluXis.Shared.Components.Users;

namespace fluXis.Shared.Utils.Extensions;

public static class UserExtensions
{
    public static bool IsDeveloper(this IAPIUserShort user)
        => user.Groups.Any(g => g.ID == "dev");

    public static bool IsPurifier(this IAPIUserShort user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "purifier");
    }

    public static bool CanModerate(this IAPIUserShort user)
    {
        if (user.IsDeveloper())
            return true;

        return user.Groups.Any(g => g.ID == "moderator");
    }
}
