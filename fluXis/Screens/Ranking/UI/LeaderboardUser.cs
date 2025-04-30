using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Overlay.User;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Ranking.UI;

public partial class LeaderboardUser : Container
{
    [Resolved]
    private UserProfileOverlay profile { get; set; }

    private APIUser user { get; }

    public LeaderboardUser(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 100;
        CornerRadius = 20;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

        if (user.Statistics == null)
            return;

        InternalChildren = new Drawable[]
        {
            new DelayedLoadUnloadWrapper(createBanner) { RelativeSizeAxes = Axes.Both },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .2f,
                        Colour = Colour4.Black.Opacity(.6f)
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.Both,
                        X = .2f,
                        Width = .8f,
                        Colour = ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.6f), Colour4.Black.Opacity(.4f))
                    }
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Width = 100,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Child = new FluXisSpriteText
                        {
                            Text = $"#{user.Statistics.GlobalRank}",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Shadow = true,
                            FontSize = 36
                        }
                    },
                    new Container
                    {
                        Width = 100,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 20,
                        Masking = true,
                        Child = new DelayedLoadUnloadWrapper(createAvatar)
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding { Left = 15 },
                        Children = new Drawable[]
                        {
                            createNames(),
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(6),
                                Children = new Drawable[]
                                {
                                    new DrawableCountry(user.GetCountry())
                                    {
                                        Width = 35,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        EdgeEffect = FluXisStyles.ShadowSmall
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = user.GetCountry().GetDescription(),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Shadow = true,
                                        WebFontSize = 20
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Vertical,
                Margin = new MarginPadding { Right = 20 },
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = $"{user.Statistics.OverallRating.ToStringInvariant("0.00")} OVR",
                        FontSize = 36,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Shadow = true
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Alpha = .8f,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = $"{user.Statistics.PotentialRating.ToStringInvariant("0.00")} PR",
                                Shadow = true
                            },
                            new FluXisSpriteText
                            {
                                Text = $"{user.Statistics.OverallAccuracy.ToStringInvariant("00.00")}%",
                                Shadow = true
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        profile.ShowUser(user.ID);
        return true;
    }

    private Drawable createBanner() => new DrawableBanner(user)
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre
    };

    private Drawable createAvatar() => new DrawableAvatar(user)
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre
    };

    private FillFlowContainer createNames()
    {
        var flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(6)
        };

        var color = FluXisColors.Text;

        if (user.Groups.Any())
        {
            var group = user.Groups.First();
            color = Colour4.TryParseHex(group.Color, out var c) ? c : color;
        }

        if (user.Club is { ID: > 0 })
        {
            flow.Add(new ClubTag(user.Club)
            {
                WebFontSize = 24,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Shadow = true
            });
        }

        if (string.IsNullOrEmpty(user.DisplayName) || user.DisplayName == user.Username)
        {
            flow.Add(new FluXisSpriteText
            {
                Text = user.Username,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = color,
                WebFontSize = 32,
                Shadow = true
            });
        }
        else
        {
            flow.AddRange(new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = user.DisplayName,
                    Colour = color,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 32,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = user.Username,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 20,
                    Alpha = .8f,
                    Shadow = true
                }
            });
        }

        return flow;
    }
}
