using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Localization;
using fluXis.Map.Drawables.Card;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network.Tabs.Shared;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardMappingTab : DashboardTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.Mapping;
    public override IconUsage Icon => FontAwesome6.Solid.PenRuler;
    public override DashboardTabType Type => DashboardTabType.Mapping;

    [Resolved]
    private IAPIClient api { get; set; }

    private FluXisScrollContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Header.Child = new DashboardRefreshButton(refresh);

        Content.Child = content = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            ScrollbarVisible = false
        };
    }

    public override void Enter()
    {
        base.Enter();
        refresh();
    }

    private void refresh()
    {
        content.Clear();

        var req = new UserMapsRequest(api.User.Value.ID);
        req.Success += res => content.Child = createContent(res.Data!);
        req.Failure += ex => content.Child = new OnlineErrorContainer
        {
            Text = ex.Message,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.Centre,
            ShowInstantly = true,
            Y = 200
        };
        api.PerformRequestAsync(req);
    }

    private static FillFlowContainer createContent(APIUserMaps maps)
    {
        var pending = maps.Impure.Where(x => x.Status == (int)MapStatus.Pending).ToList();
        var impure = maps.Impure.Where(x => x.Status != (int)MapStatus.Pending).ToList();

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(24),
            Children = new List<Drawable>
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 64,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Children = new List<Drawable>
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = "Upload Limit",
                                        WebFontSize = 16,
                                        Alpha = 0.8f,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"{maps.LimitUploadedCount} / {maps.LimitMaximumCount}",
                                        WebFontSize = 20,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Children = new List<Drawable>
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = "Queue Limit",
                                        WebFontSize = 16,
                                        Alpha = 0.8f,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"{pending.Count} / 2",
                                        WebFontSize = 20,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    }
                                }
                            }
                        }
                    }
                },
                new DashboardItemList<APIMapSet>("In Queue", pending, s => new MapCard(s) { CardWidth = 348 }) { EmptyText = "Nothing here..." },
                new DashboardItemList<APIMapSet>("Pure", maps.Pure, s => new MapCard(s) { CardWidth = 348 }) { EmptyText = "Nothing here..." },
                new DashboardItemList<APIMapSet>("Impure", impure, s => new MapCard(s) { CardWidth = 348 }) { EmptyText = "Nothing here..." },
                new DashboardItemList<APIMapSet>("Guest Difficulties", maps.Guest, s => new MapCard(s) { CardWidth = 348 }) { EmptyText = "Nothing here..." },
            }
        };
    }
}
