using osu.Framework.Graphics;

namespace fluXis.Input.Focus;

public interface IFocusable : IDrawable
{
    bool HoldingFocus => false;
    bool Activate() => false;
    void Deactivate() { }
}
