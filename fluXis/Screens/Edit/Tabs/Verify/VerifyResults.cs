using System.Collections.Generic;
using System.Linq;

namespace fluXis.Screens.Edit.Tabs.Verify;

public class VerifyResults
{
    public int TotalIssues { get; }
    public int ProblematicIssues { get; }
    public List<VerifyIssue> Issues { get; }

    public VerifyResults(List<VerifyIssue> issues)
    {
        Issues = issues;
        TotalIssues = issues.Count;
        ProblematicIssues = issues.Count(x => x.Severity == VerifyIssueSeverity.Problematic);
    }
}
