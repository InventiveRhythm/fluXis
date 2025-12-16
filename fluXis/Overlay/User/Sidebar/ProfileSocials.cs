using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Users;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.User.Sidebar;

public partial class ProfileSocials : FillFlowContainer
{
    public ProfileSocials(APIUserSocials socials)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new ForcedHeightText
            {
                Text = "Socials",
                Height = 28,
                WebFontSize = 20
            }
        };

        if (!string.IsNullOrWhiteSpace(socials.Twitter))
            AddInternal(new SocialEntry(FontAwesome.Brands.Twitter, socials.Twitter, $"https://twitter.com/{socials.Twitter}"));
        if (!string.IsNullOrWhiteSpace(socials.Twitch))
            AddInternal(new SocialEntry(FontAwesome.Brands.Twitch, socials.Twitch, $"https://twitch.tv/{socials.Twitch}"));
        if (!string.IsNullOrWhiteSpace(socials.YouTube))
            AddInternal(new SocialEntry(FontAwesome.Brands.Youtube, socials.YouTube, $"https://youtube.com/@{socials.YouTube}"));
        if (!string.IsNullOrWhiteSpace(socials.Discord))
            AddInternal(new SocialEntry(FontAwesome6.Brands.Discord, socials.Discord));

        Alpha = InternalChildren.Count > 1 ? 1f : 0f;
    }

    private partial class SocialEntry : FillFlowContainer
    {
        [CanBeNull]
        [Resolved(CanBeNull = true)]
        private FluXisGame game { get; set; }

        [CanBeNull]
        private string url { get; }

        private bool isLink => !string.IsNullOrWhiteSpace(url);

        public SocialEntry(IconUsage icon, string text, [CanBeNull] string url = null)
        {
            this.url = url;

            RelativeSizeAxes = Axes.X;
            Height = 24;
            Direction = FillDirection.Horizontal;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(20),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisSpriteText
                {
                    Text = $" {text}",
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 14
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (!isLink) return false;

            this.FadeColour(Theme.Highlight);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeColour(Colour4.White);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!isLink) return false;

            game?.OpenLink(url);
            return true;
        }
    }
}
