using System;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Drawables;
using fluXis.Online.Drawables.Users;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardOnlineTab : DashboardWipTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.Online;
    public override IconUsage Icon => FontAwesome6.Solid.EarthAmericas;
    public override DashboardTabType Type => DashboardTabType.Online;

    [Resolved]
    private IAPIClient fluxel { get; set; }

    private FluXisScrollContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Header.Child = new DashboardRefreshButton(refresh);

        Content.Children = new Drawable[]
        {
            content = new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false
            }
        };
    }

    private void refresh()
    {
        content.Clear();

        try
        {
            var req = new OnlineUsersRequest();
            req.Success += res =>
            {
                content.Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    Spacing = new Vector2(12),
                    ChildrenEnumerable = req.Response.Data.Users.Select(x => new DrawableUserCard(x)
                    {
                        Width = 346,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    })
                };
            };
            req.Failure += ex => content.Child = new OnlineErrorContainer
            {
                Text = ex.Message,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.Centre,
                ShowInstantly = true,
                Y = 200
            };
            fluxel.PerformRequestAsync(req);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to get online users");
        }
    }

    public override void Enter()
    {
        refresh();
    }
}
