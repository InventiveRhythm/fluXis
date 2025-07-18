using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Requests.Invites;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Club;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Online.Drawables.Clubs;

public partial class ClubInvitePanel : Panel, ICloseable
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private ClubOverlay clubs { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    private string code { get; }
    private APIInvite invite;

    private Container background;

    public ClubInvitePanel(string code)
    {
        this.code = code;
        Size = new Vector2(840, 580);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        StartLoading();

        var req = new GetInviteRequest(code);
        req.Success += res =>
        {
            StopLoading(false);
            invite = res.Data;

            background.AddRange(new Drawable[]
            {
                new LoadWrapper<DrawableClubBanner>
                {
                    RelativeSizeAxes = Axes.Both,
                    LoadContent = () => new DrawableClubBanner(invite.TargetClub) { RelativeSizeAxes = Axes.Both },
                    OnComplete = d => d.FadeInFromZero(300)
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2,
                    Alpha = .75f
                }
            });

            Content.Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new LoadWrapper<DrawableClubIcon>
                    {
                        Size = new Vector2(224),
                        CornerRadius = 16,
                        Masking = true,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        LoadContent = () => new DrawableClubIcon(invite.TargetClub)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        OnComplete = d => d.FadeInFromZero(300),
                        Margin = new MarginPadding { Bottom = 32 }
                    },
                    new FluXisSpriteText
                    {
                        Text = "You have been invited to join",
                        WebFontSize = 14,
                        Alpha = .8f,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Spacing = new Vector2(8),
                        Children = new Drawable[]
                        {
                            new ClubTag(invite.TargetClub)
                            {
                                WebFontSize = 20,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = invite.TargetClub.Name,
                                WebFontSize = 24,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    },
                    new FluXisButton
                    {
                        Text = "Accept Invite",
                        Size = new Vector2(240, 40),
                        Margin = new MarginPadding { Top = 24 },
                        FontSize = FluXisSpriteText.GetWebFontSize(16),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Action = acceptInvite
                    }
                }
            };
        };
        req.Failure += ex =>
        {
            Content.Child = new OnlineErrorContainer
            {
                Text = ex.Message,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            StopLoading(false);
        };
        api.PerformRequestAsync(req);
    }

    private void acceptInvite()
    {
        StartLoading();

        var req = new AcceptInviteRequest(code);
        req.Success += _ =>
        {
            StopLoading();
            clubs.ShowClub(invite.TargetClub.ID);
        };
        req.Failure += ex =>
        {
            StopLoading(false);
            panels.Replace(new SingleButtonPanel(FontAwesome6.Solid.TriangleExclamation, "Something went wrong...", ex.Message));
        };
        api.PerformRequestAsync(req);
    }

    protected override Drawable CreateBackground() => background = new ClickableContainer()
    {
        RelativeSizeAxes = Axes.Both,
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background1
            }
        }
    };

    public void Close()
    {
        if (Loading)
            return;

        Hide();
    }
}
