using fluXis.Game.Storyboards;
using NLua;

namespace fluXis.Game.Scripting.Models.Storyboarding;

public class LuaStoryboard : ILuaModel
{
    [LuaHide]
    private Storyboard storyboard { get; }

    public LuaStoryboard(Storyboard storyboard)
    {
        this.storyboard = storyboard;
    }
}
