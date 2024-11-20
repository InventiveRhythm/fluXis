using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.User;
using fluXis.Game.Utils;
using fluXis.Shared.Components.Clubs;
using fluXis.Shared.Components.Users;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Club.Tabs.Members;

public partial class ClubMemberEntry : FillFlowContainer
{
    private APIClub club { get; }
    private APIUser member { get; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ClubOverlay overlay { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay userOverlay { get; set; }

    public ClubMemberEntry(APIClub club, APIUser member)
    {
        this.member = member;
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(8);

        Children = new Drawable[]
        {
            new LoadWrapper<DrawableAvatar>
            {
                Size = new Vector2(48),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 8,
                Masking = true,
                LoadContent = () => new DrawableAvatar(member)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill
                },
                OnComplete = a => a.FadeInFromZero(300)
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Icon = FontAwesome6.Solid.Crown,
                                Size = new Vector2(16),
                                Colour = Colour4.FromHex("#FAD49E"),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Alpha = club.Owner!.ID == member.ID ? 1 : 0,
                                Margin = new MarginPadding { Right = 6 }
                            },
                            new FluXisSpriteText
                            {
                                Text = $"{member.PreferredName} ",
                                WebFontSize = 16,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                            },
                            new FluXisSpriteText
                            {
                                Text = $"{member.Username}",
                                WebFontSize = 12,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Alpha = !string.IsNullOrEmpty(member.DisplayName) ? .8f : 0
                            }
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = createOnlineStatus(),
                        WebFontSize = 10,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    }
                }
            }
        };
    }

    private string createOnlineStatus()
    {
        if (member.IsOnline)
            return "online";

        if (member.LastLogin is null)
            return "";

        var date = TimeUtils.GetFromSeconds(member.LastLogin.Value);
        return $"last online {TimeUtils.Ago(date)}";
    }

    protected override bool OnClick(ClickEvent e)
    {
        userOverlay?.ShowUser(member.ID);
        overlay?.Hide();

        return true;
    }
}
