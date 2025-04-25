namespace fluXis.Screens.Edit.Tabs.Verify;

public class VerifyIssue
{
    public VerifyIssueSeverity Severity { get; }
    public VerifyIssueCategory Category { get; }
    public double? Time { get; }
    public string Message { get; }

    public VerifyIssue(VerifyIssueSeverity severity, VerifyIssueCategory category, double? time, string message)
    {
        Severity = severity;
        Category = category;
        Time = time;
        Message = message;
    }
}

public enum VerifyIssueSeverity
{
    Hint,
    Warning,
    Problematic,
}

public enum VerifyIssueCategory
{
    Assets,
    Audio,
    Effects,
    HitObjects,
    Metadata,
}
