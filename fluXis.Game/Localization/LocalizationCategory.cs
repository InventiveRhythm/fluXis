namespace fluXis.Game.Localization;

public abstract class LocalizationCategory
{
    protected abstract string File { get; }

    protected string GetKey(string key) => $"{File}:{key.ToLower()}";
}
