using System.Linq;
using System.Net;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API;
using fluXis.Online.API.Models.Clubs;
using fluXis.Online.API.Payloads.Clubs;
using fluXis.Online.API.Requests.Clubs;
using fluXis.Overlay.Navigator.Pages.Club.Sidebar;
using fluXis.Overlay.Navigator.Pages.Club.Tabs;
using fluXis.Overlay.Navigator.Pages.Club.UI;
using fluXis.Overlay.Notifications;
using fluXis.Utils.Extensions;
using Midori.Utils;
using Midori.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.Club;

public partial class NavigatorClubPage : NavigatorPage<APIClub>
{
    public override string Path => $"/clubs/{ID}";
    protected override float ContentWidth => 1280;

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    public readonly long ID;

    public NavigatorClubPage(long id)
    {
        ID = id;
    }

    protected override APIClub PullData()
    {
        var req = new ClubRequest(ID);
        API.PerformRequest(req);
        req.ThrowIfFailed();
        return req.Response.Data;
    }

    protected override Drawable CreateContent(APIClub data) => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        AlwaysPresent = true,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(32),
        Children =
        [
            new ClubHeader(data),
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Horizontal = 16 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 32),
                        new Dimension(GridSizeMode.Absolute, 300),
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
                                RelativeSizeAxes = Axes.X,
                                Tabs = new TabContainer[]
                                {
                                    new ClubMembersTab(data),
                                    // new ClubScoresTab(),
                                }
                            },
                            Empty(),
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(28),
                                Children = new Drawable[]
                                {
                                    new FluXisButton
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 40,
                                        Text = "Edit",
                                        Action = () => editClub(data),
                                        Alpha = canEdit(data) ? 1f : 0f
                                    },
                                    new ClubSidebarStats(data),
                                    // new ClubSidebarActivity(club),
                                }
                            }
                        }
                    }
                }
            }
        ]
    };

    private bool canEdit(APIClub club)
    {
        if (API.User.Value?.CanModerate() ?? false)
            return true;

        return API.User.Value?.ID == club.Owner?.ID;
    }

    private void editClub(APIClub club)
    {
        var startData = new EditClubPayload
        {
            Name = club.Name,
            JoinType = club.JoinType,
            Icon = OnlineTextureStore.GetUrl(API, OnlineTextureStore.AssetType.ClubIcon, club.IconHash),
            Banner = OnlineTextureStore.GetUrl(API, OnlineTextureStore.AssetType.ClubBanner, club.BannerHash),
            ColorStart = club.Colors.First().Color,
            ColorEnd = club.Colors.Last().Color
        };

        panels.Content = new FormPanel<EditClubPayload>(Phosphor.Bold.PencilSimple, "Edit Club", startData.JsonCopy(), (form, data) =>
        {
            form.StartLoading();
            data = data.NullWhereSame(startData);

            var req = new EditClubRequest(club.ID, data);
            req.Success += _ => Schedule(() =>
            {
                form.StopLoading();
                form.Close();
                // TODO: reload
            });
            req.Failure += ex => Schedule(() =>
            {
                form.StopLoading();

                if (ex is APIException { Status: HttpStatusCode.NotModified })
                {
                    form.Close();
                    return;
                }

                notifications.SendError("Failed to update club!", ex.Message);
            });
            API.PerformRequestAsync(req);

            return false;
        });
    }
}
