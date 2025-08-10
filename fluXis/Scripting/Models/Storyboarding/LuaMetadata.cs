using fluXis.Map;
using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding;

[LuaDefinition("storyboard", Name = "metadata", Public = true)]
public class LuaMetadata : ILuaModel
{
    /// <summary>
    /// the non-romanized title of the current map
    /// </summary>
    [LuaMember(Name = "title")]
    public string Title { get; }

    /// <summary>
    /// the non-romanized artist of the current map
    /// </summary>
    [LuaMember(Name = "artist")]
    public string Artist { get; }

    [LuaMember(Name = "mapper")]
    public string Mapper { get; }

    /// <summary>
    /// difficulty name of the current map
    /// </summary>
    [LuaMember(Name = "difficulty")]
    public string Difficulty { get; }

    /// <summary>
    /// relative path to the background image
    /// </summary>
    [LuaMember(Name = "background")]
    public string Background { get; }

    /// <summary>
    /// relative path to the cover image
    /// </summary>
    [LuaMember(Name = "cover")]
    public string Cover { get; }

    public LuaMetadata(MapInfo map)
    {
        Title = map.Metadata.Title;
        Artist = map.Metadata.Artist;
        Mapper = map.Metadata.Mapper;
        Difficulty = map.Metadata.Difficulty;
        Background = map.BackgroundFile;
        Cover = map.CoverFile;
    }
}
