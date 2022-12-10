namespace fluXis.Game.Utils
{
    public class MathUtils
    {
        public static float DeltaLerp(double from, double to, double delta)
        {
            return DeltaLerp((float)from, (float)to, (float)delta);
        }

        public static float DeltaLerp(float from, float to, float delta)
        {
            return from + (to - from) * delta;
        }
    }
}
