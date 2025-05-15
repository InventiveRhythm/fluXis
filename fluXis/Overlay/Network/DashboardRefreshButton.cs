using System;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Localization;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Overlay.Network;

public partial class DashboardRefreshButton : FluXisButton
{
    public DashboardRefreshButton(Action action)
    {
        Text = LocalizationStrings.General.Refresh;
        FontSize = 20;
        Size = new Vector2(144, 36);
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        Margin = new MarginPadding { Right = 20 };
        Color = Colour4.Transparent;
        Action = action;
    }
}
