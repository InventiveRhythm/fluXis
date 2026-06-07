using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Overlay.User;
using fluXis.Scoring;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Result.Header;

public partial class ResultsPlayer : CompositeDrawable
{
    [Resolved]
    private Bindable<ScoreInfo> score { get; set; }

    [Resolved]
    private Bindable<APIUser> user { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay overlay { get; set; }

    private LoadWrapper<DrawableAvatar> avatar;
    private ClubTag club;
    private FluXisSpriteText username;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        var date = TimeUtils.GetFromSeconds(score.Value.Timestamp);

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(12),
            Children = new Drawable[]
            {
                avatar = new LoadWrapper<DrawableAvatar>
                {
                    Size = new Vector2(88),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    EdgeEffect = Styling.ShadowMedium,
                    CornerRadius = 12,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(user.Value)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        ShowTooltip = true
                    }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Children = new Drawable[]
                            {
                                club = new ClubTag(user.Value.Club)
                                {
                                    WebFontSize = 16,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Shadow = true
                                },
                                username = new FluXisSpriteText
                                {
                                    Text = $" {user.Value.Username}",
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    WebFontSize = 20,
                                    Shadow = true
                                }
                            }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"{date.Day} {date:MMM} {date.Year} @ {date:HH:mm}",
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            WebFontSize = 14,
                            Shadow = true,
                            Alpha = .8f
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        user.BindValueChanged(v =>
        {
            avatar.Reload();
            club.ChangeClub(v.NewValue.Club);
            username.Text = $" {user.Value.Username}";
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (user.Value.ID > 0) overlay?.ShowUser(user.Value.ID);
        return true;
    }
}
