using System.Collections.Generic;
using JetBrains.Annotations;

namespace fluXis.Screens.Edit.Tabs.Verify;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IVerifyCheck
{
    IEnumerable<VerifyIssue> Check(EditorMap map);
}
