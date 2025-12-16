using System;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.User.Header;

public partial class HeaderFollowButton : CompositeDrawable
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private APIUser user { get; }
    private bool following => user.Following is >= UserFollowState.Following;

    private Box background;
    private FlashLayer flash;
    private FillFlowContainer flow;
    private SpriteIcon icon;
    private FluXisSpriteText text;

    public HeaderFollowButton(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 160;
        Height = 50;
        CornerRadius = 25;
        Masking = true;
        EdgeEffect = Styling.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = following ? Theme.Primary : Theme.Background2,
            },
            flash = new FlashLayer(),
            flow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Colour = following ? Theme.Background2 : Theme.Text,
                Padding = new MarginPadding
                {
                    Horizontal = 20
                },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(20),
                        Child = icon = new FluXisSpriteIcon
                        {
                            Y = 2, // it looks kinda off-center without this
                            Icon = FontAwesome6.Solid.Heart,
                            Size = new Vector2(20)
                        }
                    },
                    text = new FluXisSpriteText
                    {
                        Text = following ? "Following" : "Follow",
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.Show();

        var req = new UserFollowRequest(user.ID, following);
        api.PerformRequestAsync(req);

        user.Following = user.Following switch
        {
            UserFollowState.None => UserFollowState.Following,
            UserFollowState.Followed => UserFollowState.Mutual,
            UserFollowState.Following => UserFollowState.None,
            UserFollowState.Mutual => UserFollowState.Followed,
            _ => throw new ArgumentOutOfRangeException()
        };

        text.Text = following ? "Unfollow" : "Follow";
        background.FadeColour(following ? Theme.Red : Theme.Primary, 200);
        icon.Icon = following ? FontAwesome6.Solid.HeartCrack : FontAwesome6.Solid.Heart;

        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();

        flow.FadeColour(Theme.Background2, 50);

        if (following)
        {
            background.FadeColour(Theme.Red, 50);
            icon.Icon = FontAwesome6.Solid.HeartCrack;
            icon.Vibrate(100, 4);
            text.Text = "Unfollow";
        }
        else
            background.FadeColour(Theme.Primary, 50);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        icon.Icon = FontAwesome6.Solid.Heart;
        text.Text = following ? "Following" : "Follow";

        if (following)
            background.FadeColour(Theme.Primary, 200);
        else
        {
            background.FadeColour(Theme.Background2, 200);
            flow.FadeColour(Theme.Text, 200);
        }

        base.OnHoverLost(e);
    }
}
