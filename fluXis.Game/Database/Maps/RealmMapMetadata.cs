using JetBrains.Annotations;
using Realms;

namespace fluXis.Game.Database.Maps;

public class RealmMapMetadata : RealmObject
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Audio { get; set; } = string.Empty;
    public int PreviewTime { get; set; } = 0;

    [UsedImplicitly]
    public RealmMapMetadata()
    {
    }

    public override string ToString()
    {
        return $"{Title} - {Artist} - {Mapper} - {Source} - {Tags} - {Background} - {Audio} - {PreviewTime}";
    }
}
