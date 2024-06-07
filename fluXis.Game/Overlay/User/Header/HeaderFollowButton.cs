using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Utils.Extensions;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.User.Header;

public partial class HeaderFollowButton : CompositeDrawable
{
    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private APIUser user { get; }
    private bool following => user.Following!.Value;

    private Box background;
    private Box flash;
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
        AutoSizeAxes = Axes.X;
        Height = 50;
        CornerRadius = 25;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = following ? FluXisColors.Accent2 : FluXisColors.Background2,
            },
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            flow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Colour = following ? FluXisColors.Background2 : FluXisColors.Text,
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
                        Child = icon = new SpriteIcon
                        {
                            Y = 2, // it looks kinda off-center without this
                            Icon = FontAwesome6.Solid.Heart,
                            Size = new Vector2(20)
                        }
                    },
                    text = new FluXisSpriteText
                    {
                        Text = user.Following!.Value ? "Following" : "Follow",
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);

        var req = new UserFollowRequest(user.ID, following);
        fluxel.PerformRequestAsync(req);

        user.Following = !following;
        text.Text = user.Following!.Value ? "Unfollow" : "Follow";
        background.FadeColour(user.Following!.Value ? FluXisColors.Red : FluXisColors.Accent2, 200);
        icon.Icon = user.Following!.Value ? FontAwesome6.Solid.HeartCrack : FontAwesome6.Solid.Heart;

        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();

        flow.FadeColour(FluXisColors.Background2, 50);

        if (following)
        {
            background.FadeColour(FluXisColors.Red, 50);
            icon.Icon = FontAwesome6.Solid.HeartCrack;
            icon.Vibrate(100, 4);
            text.Text = "Unfollow";
        }
        else
            background.FadeColour(FluXisColors.Accent2, 50);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        icon.Icon = FontAwesome6.Solid.Heart;
        text.Text = user.Following!.Value ? "Following" : "Follow";

        if (following)
            background.FadeColour(FluXisColors.Accent2, 200);
        else
        {
            background.FadeColour(FluXisColors.Background2, 200);
            flow.FadeColour(FluXisColors.Text, 200);
        }

        base.OnHoverLost(e);
    }
}
