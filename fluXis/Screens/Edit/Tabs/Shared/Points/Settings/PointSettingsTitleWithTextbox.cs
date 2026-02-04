using System;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings;

public partial class PointSettingsTitleWithTextbox : PointSettingsTitle
{
    public PointSettingsTitleWithTextbox(string title, Action deleteAction, bool showWiki = true)
        : base(title, deleteAction, showWiki)
    {
    }
}
