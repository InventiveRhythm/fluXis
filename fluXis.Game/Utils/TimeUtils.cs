namespace fluXis.Game.Utils;

public class TimeUtils
{
    public static string Format(float time)
    {
        time = time / 1000;

        int hours = (int)time / 3600;
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        int milliseconds = (int)(time * 1000) % 1000;

        string timeString = "";

        if (hours > 0)
            timeString += hours.ToString("00") + ":";

        timeString += $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        return timeString;
    }
}
