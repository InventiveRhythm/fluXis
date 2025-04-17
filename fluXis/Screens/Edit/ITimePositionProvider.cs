using osuTK;

namespace fluXis.Screens.Edit;

public interface ITimePositionProvider
{
    float PositionAtTime(double time);

    double TimeAtScreenSpacePosition(Vector2 pos);
    Vector2 ScreenSpacePositionAtTime(double time, int lane);
}
