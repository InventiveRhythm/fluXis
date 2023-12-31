using System;
using fluXis.Game.Localization;
using osu.Framework.Localisation;

namespace fluXis.Game.Utils;

public static class LocaleUtils
{
    public static string ToCultureCode(this Language lang) => lang.ToString().Replace("_", "-");

    public static bool TryParseCultureCode(string cultureCode, out Language language) => Enum.TryParse(cultureCode.Replace("-", "_"), out language);

    public static Language GetLanguageFor(string locale, LocalisationParameters localeParams)
    {
        if (TryParseCultureCode(locale, out var language))
            return language;

        if (localeParams.Store == null)
            return Language.en;

        return TryParseCultureCode(localeParams.Store.EffectiveCulture.Name, out language) ? language : Language.en;
    }
}
