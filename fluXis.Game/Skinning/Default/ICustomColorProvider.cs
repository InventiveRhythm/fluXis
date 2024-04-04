using osu.Framework.Graphics;

namespace fluXis.Game.Skinning.Default;

public interface ICustomColorProvider
{
    Colour4 Primary { get; }
    Colour4 Secondary { get; }
    Colour4 Middle { get; }

    bool HasColorFor(int lane, int keyCount, out Colour4 colour);
    Colour4 GetColor(int index);
}
