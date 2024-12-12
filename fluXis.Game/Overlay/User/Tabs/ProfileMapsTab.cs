using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Tabs;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User.Tabs.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.User.Tabs;

public partial class ProfileMapsTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.Map;
    public override string Title => "Maps";

    [Resolved]
    private IAPIClient api { get; set; }

    private APIUser user { get; }

    private FillFlowContainer flow;
    private LoadingIcon loading;
    private FluXisSpriteText error;

    private ProfileMapsSection pure;
    private ProfileMapsSection impure;
    private ProfileMapsSection guest;

    public ProfileMapsTab(APIUser user)
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
                    pure = new ProfileMapsSection("Pure"),
                    impure = new ProfileMapsSection("Impure/Unsubmitted"),
                    guest = new ProfileMapsSection("Guest Difficulties"),
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

        var req = new UserMapsRequest(user.ID);
        req.Success += populate;
        req.Failure += displayError;
        api.PerformRequestAsync(req);
    }

    private void populate(APIResponse<APIUserMaps> res)
    {
        if (!res.Success)
            return;

        pure.Maps = res.Data.Pure;
        impure.Maps = res.Data.Impure;
        guest.Maps = res.Data.Guest;

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
