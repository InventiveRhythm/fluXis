using System.ComponentModel;
using fluXis.Utils.Attributes;

namespace fluXis.Screens.Edit.UI.Panels;

public class EditorApplyGroup
{
    [Description("Target Group")]
    [Placeholder("...")]
    public string Group { get; set; }
}
