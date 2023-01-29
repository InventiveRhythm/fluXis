using System;

namespace fluXis.Game.Utils;

public class TimeUtils
{
    public static string Format(float time, bool showMs = true)
    {
        time /= 1000;

        bool negative = time < 0;
        if (negative)
            time = Math.Abs(time);

        int hours = (int)time / 3600;
        int minutes = (int)time / 60 % 60;
        int seconds = (int)time % 60;
        int milliseconds = (int)(time * 1000) % 1000;

        string timeString = "";

        if (negative)
            timeString += "-";

        if (hours > 0)
            timeString += hours.ToString("00") + ":";

        timeString += $"{minutes:00}:{seconds:00}";

        if (showMs)
            timeString += $".{milliseconds:000}";

        return timeString;
    }
}
