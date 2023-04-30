using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Profile;

public partial class SocialChip : Container
{
    public UserSocialType Type { get; set; }
    public string Username { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 5;

        Colour4 colour = Type switch
        {
            UserSocialType.Twitter => FluXisColors.SocialTwitter,
            UserSocialType.Youtube => FluXisColors.SocialYoutube,
            UserSocialType.Twitch => FluXisColors.SocialTwitch,
            UserSocialType.Discord => FluXisColors.SocialDiscord,
            _ => Colour4.White
        };

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = colour.Opacity(.5f)
            },
            new FillFlowContainer
            {
                Margin = new MarginPadding(5),
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Size = new Vector2(20),
                        Icon = Type switch
                        {
                            UserSocialType.Twitter => FontAwesome.Brands.Twitter,
                            UserSocialType.Youtube => FontAwesome.Brands.Youtube,
                            UserSocialType.Twitch => FontAwesome.Brands.Twitch,
                            UserSocialType.Discord => FontAwesome.Brands.Discord,
                            _ => FontAwesome.Solid.Question
                        },
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new SpriteText
                    {
                        Text = Username,
                        Font = FluXisFont.Default(23),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            }
        };
    }
}

public enum UserSocialType
{
    Twitter,
    Youtube,
    Twitch,
    Discord
}
