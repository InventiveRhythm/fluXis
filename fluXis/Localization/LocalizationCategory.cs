using osu.Framework.Localisation;

namespace fluXis.Localization;

public abstract class LocalizationCategory
{
    public string FileName => File;

    protected abstract string File { get; }

    protected string GetKey(string key) => $"{File}:{key.ToLower()}";
    protected TranslatableString Get(string key, string defaultStr) => new(GetKey(key), defaultStr);
}
