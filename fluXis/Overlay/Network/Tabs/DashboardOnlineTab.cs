using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardOnlineTab : DashboardWipTab
{
    public override string Title => "Online";
    public override IconUsage Icon => FontAwesome6.Solid.EarthAmericas;
    public override DashboardTabType Type => DashboardTabType.Online;

    [Resolved]
    private IAPIClient fluxel { get; set; }

    private bool visible;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Header.Child = new FluXisButton
        {
            Text = "Refresh",
            FontSize = 20,
            Size = new Vector2(144, 36),
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Margin = new MarginPadding { Right = 20 },
            Color = Colour4.Transparent,
            Action = refresh
        };

        Content.Child = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(12)
            }
        };
    }

    private void refresh()
    {
        flow.Clear();

        try
        {
            var req = new OnlineUsersRequest();
            fluxel.PerformRequest(req);

            if (req.IsSuccessful)
            {
                Schedule(() =>
                {
                    if (!visible)
                        return;

                    foreach (var user in req.Response.Data.Users)
                    {
                        flow.Add(new DrawableUserCard(user)
                        {
                            Width = 346,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        });
                    }
                });
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to get online users");
        }
    }

    public override void Enter()
    {
        visible = true;
        refresh();
    }

    public override void Exit()
    {
        visible = false;
    }
}
