using osuTK;

namespace fluXis.Screens.Edit;

public interface ITimePositionProvider
{
    double TimeAtScreenSpacePosition(Vector2 pos);
    Vector2 ScreenSpacePositionAtTime(double time, int lane);
}
