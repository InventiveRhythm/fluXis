using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Online.API;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.User.Tabs.Scores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.User.Tabs;

public partial class ProfileScoresTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.ArrowTrendUp;
    public override string Title => "Scores";

    [Resolved]
    private IAPIClient api { get; set; }

    private APIUser user { get; }

    private FillFlowContainer flow;
    private LoadingIcon loading;
    private FluXisSpriteText error;

    private ProfileScoresSection best;
    private ProfileScoresSection recent;

    public ProfileScoresTab(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(20),
                Direction = FillDirection.Vertical,
                Alpha = 0,
                Children = new Drawable[]
                {
                    best = new ProfileScoresSection("Best"),
                    recent = new ProfileScoresSection("Recent"),
                }
            },
            error = new FluXisSpriteText
            {
                WebFontSize = 14,
                Margin = new MarginPadding { Vertical = 32 },
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            loading = new LoadingIcon
            {
                Size = new Vector2(32),
                Margin = new MarginPadding { Vertical = 32 },
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var req = new UserScoresRequest(user.ID);
        req.Success += populate;
        req.Failure += displayError;
        api.PerformRequestAsync(req);
    }

    private void populate(APIResponse<APIUserScores> res)
    {
        if (!res.Success)
            return;

        best.Maps = res.Data.Best;
        recent.Maps = res.Data.Recent;

        flow.FadeIn(300);
        loading.Hide();
    }

    private void displayError(Exception exception)
    {
        error.Text = exception.Message;
        loading.Hide();
        error.FadeIn(300);
    }
}
