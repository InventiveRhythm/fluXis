using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Requests.MapSets;
using fluXis.Overlay.Navigator.Pages.MapSet.Buttons;
using fluXis.Overlay.Navigator.Pages.MapSet.Sidebar;
using fluXis.Overlay.Navigator.Pages.MapSet.Tabs;
using fluXis.Overlay.Navigator.Pages.MapSet.UI.Difficulties;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.MapSet;

public partial class NavigatorMapSetPage : NavigatorPage<APIMapSet>
{
    public override string Path => $"/mapsets/{ID}";
    protected override float ContentWidth => 1280;

    public readonly long ID;

    public NavigatorMapSetPage(long id)
    {
        ID = id;
    }

    protected override APIMapSet PullData()
    {
        var req = new MapSetRequest(ID);
        API.PerformRequest(req);
        req.ThrowIfFailed();
        return req.Response.Data;
    }

    protected override Drawable CreateContent(APIMapSet data)
    {
        data.Maps.Sort((a, b) => a.Rating.CompareTo(b.Rating));
        var bindableMap = new Bindable<APIMap>(data.Maps.First());

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            AlwaysPresent = true,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(20),
            Children =
            [
                new MapSetHeader(data),
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Horizontal = 10, Bottom = 20 },
                    Spacing = new Vector2(20),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Spacing = new Vector2(12),
                                    ChildrenEnumerable = data.Maps.Select(m => new DifficultyChip(data, m, bindableMap))
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Spacing = new Vector2(12),
                                    Children = new Drawable[]
                                    {
                                        new MapSetFavoriteButton(data),
                                        new MapSetButton(Phosphor.Bold.ShareNetwork, ()
                                            => Game?.OpenLink($"{API.Endpoint.WebsiteRootUrl}/set/{data.ID}#{bindableMap.Value.ID}", ingame: false)),
                                        new MapSetDownloadButton(data),
                                        // new MapSetButton(FontAwesome6.Solid.EllipsisVertical, () => { })
                                    }
                                }
                            }
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            ColumnDimensions = new[]
                            {
                                new Dimension(),
                                new Dimension(GridSizeMode.Absolute, 20),
                                new Dimension(GridSizeMode.Absolute, 320)
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new[]
                                {
                                    new TabControl
                                    {
                                        Tabs = createTabs(data, bindableMap).ToArray()
                                    },
                                    Empty(),
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(20),
                                        Children = new Drawable[]
                                        {
                                            new MapSetSidebarMapper(bindableMap),
                                            new MapSetSidebarVoting(data),
                                            new MapSetSidebarStats(bindableMap)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ]
        };
    }

    private IEnumerable<TabContainer> createTabs(APIMapSet set, Bindable<APIMap> bindableMap)
    {
        // yield return new MapSetInfoTab();
        yield return new MapSetScoreTab(bindableMap);

        if (set.ShowModActions)
            yield return new MapSetModdingTab(set);

        // yield return new MapSetCommentsTab();
    }
}
