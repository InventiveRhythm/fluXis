using fluXis.Skinning.Bases;
using osu.Framework.Graphics;

namespace fluXis.Skinning.Default;

public interface ICustomColorProvider
{
    Colour4 Primary { get; }
    Colour4 Secondary { get; }
    Colour4 Middle { get; }

    bool HasColorFor(int lane, int keyCount, out Colour4 colour);
    Colour4 GetColor(int index, Colour4 fallback);

    void Register(ColorableSkinDrawable skinDrawable) { }
    void Unregister(ColorableSkinDrawable skinDrawable) { }
}
