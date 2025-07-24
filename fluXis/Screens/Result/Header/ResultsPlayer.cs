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
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Result.Header;

public partial class ResultsPlayer : CompositeDrawable
{
    [Resolved]
    private ScoreInfo score { get; set; }

    [Resolved]
    private APIUser user { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay overlay { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        var date = TimeUtils.GetFromSeconds(score.Timestamp);

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(12),
            Children = new Drawable[]
            {
                new LoadWrapper<DrawableAvatar>
                {
                    Size = new Vector2(88),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    EdgeEffect = Styling.ShadowMedium,
                    CornerRadius = 12,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(user)
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
                                new ClubTag(user.Club)
                                {
                                    WebFontSize = 16,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Shadow = true
                                },
                                new FluXisSpriteText
                                {
                                    Text = $" {user.Username}",
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

    protected override bool OnClick(ClickEvent e)
    {
        overlay?.ShowUser(user.ID);
        return true;
    }
}
