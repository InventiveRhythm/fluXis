using fluXis.Game.Skinning.Default;

namespace fluXis.Game.Skinning;

public class SkinManager
{
    public Skin CurrentSkin { get; private set; }

    public SkinManager()
    {
        CurrentSkin = new DefaultSkin();
    }
}
