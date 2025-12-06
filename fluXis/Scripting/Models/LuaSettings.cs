using fluXis.Configuration;
using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Scripting.Models;

// in storyboard for now, until effect scripting gets worked on more
[LuaDefinition("storyboard", Name = "settings", Public = true)]
public class LuaSettings : ILuaModel
{
    [LuaMember(Name = "scrollspeed")]
    public float ScrollSpeed { get; }

    [LuaMember(Name = "upscroll")]
    public bool UpScroll { get; }

    public LuaSettings(FluXisConfig config)
    {
        ScrollSpeed = config.Get<float>(FluXisSetting.ScrollSpeed);
        UpScroll = config.Get<ScrollDirection>(FluXisSetting.ScrollDirection) == ScrollDirection.Up;
    }
}
