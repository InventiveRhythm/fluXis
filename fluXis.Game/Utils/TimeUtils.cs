using System;

namespace fluXis.Game.Utils;

public class TimeUtils
{
    public static string Format(float time, bool showMs = true)
    {
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

    public static string Ago(DateTimeOffset time)
    {
        TimeSpan span = DateTimeOffset.Now - time;

        if (span.TotalDays > 365)
        {
            int years = (int)(span.TotalDays / 365);
            return years == 1 ? "1 year ago" : $"{years} years ago";
        }

        if (span.TotalDays > 30)
        {
            int months = (int)(span.TotalDays / 30);
            return months == 1 ? "1 month ago" : $"{months} months ago";
        }

        if (span.TotalDays > 1)
        {
            int days = (int)span.TotalDays;
            return days == 1 ? "1 day ago" : $"{days} days ago";
        }

        if (span.TotalHours > 1)
        {
            int hours = (int)span.TotalHours;
            return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
        }

        if (span.TotalMinutes > 1)
        {
            int minutes = (int)span.TotalMinutes;
            return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
        }

        if (span.TotalSeconds >= 30)
            return $"{(int)span.TotalSeconds} seconds ago";

        return "now";
    }
}
