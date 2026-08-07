namespace fluXis.Map.Structures.Bases;

public interface IHasDuration : ITimedObject
{
    double Duration { get; set; }
}

public static class HasDurationExtensions
{
    public static double GetEndTime(this IHasDuration dur) => dur.Time + dur.Duration;
}
