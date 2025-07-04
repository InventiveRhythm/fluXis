using fluXis.Map;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding;

public class LuaMetadata : ILuaModel
{
    [LuaMember(Name = "title")]
    public string Title { get; }

    [LuaMember(Name = "artist")]
    public string Artist { get; }

    [LuaMember(Name = "mapper")]
    public string Mapper { get; }

    [LuaMember(Name = "difficulty")]
    public string Difficulty { get; }

    [LuaMember(Name = "background")]
    public string Background { get; }

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
