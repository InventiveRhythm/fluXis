using System;
using fluXis.Online;
using fluXis.Online.API.Models.Users;

namespace fluXis.Utils;

public static class CountryUtils
{
    public static CountryCode GetCountryCode(string code)
    {
        if (code == null)
            return CountryCode.Unknown;

        if (Enum.TryParse<CountryCode>(code, true, out var countryCode))
            return countryCode;

        return CountryCode.Unknown;
    }

    public static CountryCode GetCountry(this APIUser user) => GetCountryCode(user.CountryCode);
}
