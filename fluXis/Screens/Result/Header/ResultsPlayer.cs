using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
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
    private Results results { get; set; }

    private APIUser user;
    private int playerIndex;

    private FluXisSpriteText usernameText;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay overlay { get; set; }

    public ResultsPlayer(APIUser user, int playerIndex)
    {
        this.user = user;
        this.playerIndex = playerIndex;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        var date = TimeUtils.GetFromSeconds(score.Timestamp);

        var color = (playerIndex == results.SelectedPlayer.Value) ? Theme.Purple : Theme.Text;

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
                                usernameText = new FluXisSpriteText
                                {
                                    Text = $" {user.Username}",
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Colour = color,
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

        results.SelectedPlayer.BindValueChanged(v => onResultSelectedPlayerChange(v.NewValue));
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (results.SelectedPlayer.Value == playerIndex) overlay?.ShowUser(user.ID);
        else results.SelectedPlayer.Value = playerIndex;
        return true;
    }

    private void onResultSelectedPlayerChange(int newPlayerIndex)
    {
        usernameText.Colour = newPlayerIndex == playerIndex ? Theme.Purple : Theme.Text;
    }
}
