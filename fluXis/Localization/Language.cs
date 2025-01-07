using System.ComponentModel;

namespace fluXis.Localization;

// Resharper disable InconsistentNaming
public enum Language
{
    [Description("English")]
    en,

    [Description("Dansk")]
    da,

    [Description("Deutsch")]
    de,

    [Description("español")]
    es_ES,

    [Description("Suomi")]
    fi,

    [Description("français")]
    fr,

    [Description("Hrvatski")]
    hr,

    [Description("magyar")]
    hu,

    [Description("Italiano")]
    it,

    [Description("日本語")]
    ja,

    [Description("한국어")]
    ko,

    [Description("LOLCAT")]
    lol,

    [Description("Nederlands")]
    nl,

    [Description("Norsk")]
    no,

    [Description("polski")]
    pl,

    [Description("Русский")]
    ru,

    [Description("Svenska")]
    sv_SE,

    [Description("Tagalog")]
    tl,

    [Description("Türkçe")]
    tr,

    [Description("简体中文")]
    zh_CN,

    [Description("繁體中文（台灣）")]
    zh_TW,

    [Description("Debug (raw keys)")]
    debug
}
