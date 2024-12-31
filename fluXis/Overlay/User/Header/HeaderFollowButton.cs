using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
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
    private bool following => user.Following!.Value;

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
                Colour = following ? FluXisColors.Primary : FluXisColors.Background2,
            },
            flash = new FlashLayer(),
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
                        Child = icon = new FluXisSpriteIcon
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
        flash.Show();

        var req = new UserFollowRequest(user.ID, following);
        api.PerformRequestAsync(req);

        user.Following = !following;
        text.Text = user.Following!.Value ? "Unfollow" : "Follow";
        background.FadeColour(user.Following!.Value ? FluXisColors.Red : FluXisColors.Primary, 200);
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
            background.FadeColour(FluXisColors.Primary, 50);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        icon.Icon = FontAwesome6.Solid.Heart;
        text.Text = user.Following!.Value ? "Following" : "Follow";

        if (following)
            background.FadeColour(FluXisColors.Primary, 200);
        else
        {
            background.FadeColour(FluXisColors.Background2, 200);
            flow.FadeColour(FluXisColors.Text, 200);
        }

        base.OnHoverLost(e);
    }
}
