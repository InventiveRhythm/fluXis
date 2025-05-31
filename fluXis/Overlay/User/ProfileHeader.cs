using System.Linq;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Groups;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Fluxel;
using fluXis.Overlay.User.Header;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.User;

public partial class ProfileHeader : Container
{
    private APIUser user { get; }

    private bool showUsername => user.Username != user.PreferredName;
    private bool showPronouns => !string.IsNullOrEmpty(user.Pronouns);
    private bool showBottomRow => showUsername || showPronouns;

    public ProfileHeader(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader(true)]
    private void load(IAPIClient api, [CanBeNull] FluXisGame game)
    {
        RelativeSizeAxes = Axes.X;
        Height = 420; // this is changed in Update()
        CornerRadius = 24;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new LoadWrapper<DrawableBanner>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableBanner(user)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = banner => banner.FadeInFromZero(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2.Opacity(.5f)
            },
            new FillFlowContainer
            {
                Width = 1200,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(20),
                Padding = new MarginPadding { Horizontal = 40 },
                Children = new Drawable[]
                {
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 128,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 128),
                            new Dimension(GridSizeMode.Absolute, 12),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                new LoadWrapper<DrawableAvatar>
                                {
                                    Size = new Vector2(128),
                                    CornerRadius = 24,
                                    Masking = true,
                                    EdgeEffect = FluXisStyles.ShadowMedium,
                                    LoadContent = () => new DrawableAvatar(user)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre
                                    },
                                    OnComplete = avatar => avatar.FadeInFromZero(400)
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Children = new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Children = new[]
                                            {
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Direction = FillDirection.Horizontal,
                                                    Spacing = new Vector2(12),
                                                    ChildrenEnumerable = user.Groups.Any()
                                                        ? user.Groups.Select(g => new HeaderGroupChip(g))
                                                        : new Drawable[]
                                                        {
                                                            new HeaderGroupChip(new APIGroup
                                                            {
                                                                ID = "member",
                                                                Name = "Member",
                                                                Color = "#AA99FF"
                                                            })
                                                        }
                                                },
                                                getOnlineStatus().With(d =>
                                                {
                                                    d.Anchor = Anchor.CentreRight;
                                                    d.Origin = Anchor.CentreRight;
                                                })
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Horizontal,
                                            Spacing = new Vector2(12),
                                            Children = new Drawable[]
                                            {
                                                new ClubTag(user.Club)
                                                {
                                                    Alpha = user.Club?.ID > 0 ? 1 : 0,
                                                    Shadow = true,
                                                    WebFontSize = 30,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                },
                                                new ClickableContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Child = getName(user.PreferredName),
                                                    Action = () => game?.OpenLink($"{api.Endpoint.WebsiteRootUrl}/u/{user.ID}"),
                                                }
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Horizontal,
                                            Spacing = new Vector2(12),
                                            Alpha = showBottomRow ? 1 : 0,
                                            Margin = new MarginPadding { Top = -8 },
                                            Children = new Drawable[]
                                            {
                                                new FluXisTooltipText
                                                {
                                                    Text = user.Username,
                                                    TooltipText = "Username",
                                                    WebFontSize = 24,
                                                    Shadow = true,
                                                    Alpha = showUsername ? .8f : 0,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                },
                                                new FluXisTooltipText
                                                {
                                                    Text = user.Pronouns,
                                                    TooltipText = "Pronouns",
                                                    WebFontSize = 20,
                                                    Shadow = true,
                                                    Alpha = showPronouns ? .6f : 0,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                AutoSizeDuration = 400,
                                AutoSizeEasing = Easing.OutQuint,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Children = new Drawable[]
                                {
                                    new HeaderPlacementChip
                                    {
                                        Placement = user.Statistics!.GlobalRank,
                                        CreateIcon = () => new FluXisSpriteIcon
                                        {
                                            Icon = FontAwesome6.Solid.EarthAmericas,
                                            Size = new Vector2(20)
                                        }
                                    },
                                    new HeaderPlacementChip
                                    {
                                        Placement = user.Statistics!.CountryRank,
                                        CreateIcon = () => new DrawableCountry(user.GetCountry())
                                        {
                                            Size = new Vector2(20)
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                AutoSizeDuration = 400,
                                AutoSizeEasing = Easing.OutQuint,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Children = new Drawable[]
                                {
                                    new HeaderButton
                                    {
                                        Icon = FontAwesome6.Solid.ShareNodes
                                    },
                                    api.User.Value?.ID == user.ID
                                        ? new HeaderEditButton()
                                        : new HeaderFollowButton(user)
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        Height = DrawWidth / 3f; // 3:1 aspect ratio
    }

    private Drawable getName(string text)
    {
        var displayName = text != user.Username;
        var tooltipText = displayName ? "Display Name" : "Username";

        if (user.NamePaint is null)
        {
            return new FluXisTooltipText
            {
                Text = text,
                WebFontSize = 48,
                TooltipText = tooltipText,
                Shadow = true
            };
        }

        return new GradientText
        {
            Text = text,
            WebFontSize = 48,
            Shadow = true,
            TooltipText = tooltipText,
            Colour = user.NamePaint.Colors.CreateColorInfo()
        };
    }

    private Drawable getOnlineStatus()
    {
        if (user.IsOnline)
        {
            return new FluXisSpriteText
            {
                Text = "Currently Online",
                Shadow = true,
                WebFontSize = 16
            };
        }

        return new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(5),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Last Online",
                    Shadow = true,
                    WebFontSize = 16,
                    Alpha = .8f
                },
                new FluXisSpriteText
                {
                    Text = TimeUtils.Ago(TimeUtils.GetFromSeconds(user.LastLogin ?? 0)),
                    Shadow = true,
                    WebFontSize = 16
                }
            }
        };
    }
}
