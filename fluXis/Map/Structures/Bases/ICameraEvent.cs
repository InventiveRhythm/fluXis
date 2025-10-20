using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Bases;

public interface ICameraEvent : IMapEvent
{
    void Apply(Drawable draw);
}
