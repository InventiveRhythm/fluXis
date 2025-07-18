using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Overlay.Mouse;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Online.Drawables.Users;

public partial class UserTooltip : CustomTooltipContainer<APIUser>
{
    private Container banner { get; }
    private Container avatar { get; }
    private FillFlowContainer topText { get; }
    private ForcedHeightText bottomText { get; }

    private FluXisSpriteIcon statusIcon;
    private FluXisSpriteText statusText;

    private long lastId = -1;

    public UserTooltip()
    {
        AutoSizeAxes = Axes.Y;
        Width = 300;
        CornerRadius = 8;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 56,
                        CornerRadius = 8,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            banner = new Container { RelativeSizeAxes = Axes.Both },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Theme.Background2,
                                Alpha = .5f
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(8),
                                Children = new Drawable[]
                                {
                                    avatar = new Container
                                    {
                                        Size = new Vector2(56),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(4),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Children = new Drawable[]
                                        {
                                            topText = new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.X,
                                                Height = 14,
                                                Direction = FillDirection.Horizontal,
                                                Spacing = new Vector2(4)
                                            },
                                            bottomText = new ForcedHeightText()
                                            {
                                                Height = 14,
                                                WebFontSize = 12,
                                                Alpha = .8f,
                                                Shadow = true
                                            }
                                        }
                                    }
                                },
                            }
                        }
                    },
                    new FillFlowContainer()
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 32,
                        Padding = new MarginPadding { Horizontal = 12 },
                        Spacing = new Vector2(4),
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            statusIcon = new FluXisSpriteIcon
                            {
                                Size = new Vector2(12),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            statusText = new FluXisSpriteText
                            {
                                WebFontSize = 10,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };
    }

    public override void SetContent(APIUser user)
    {
        if (user.ID == lastId)
            return;

        lastId = user.ID;

        banner.Child = new LoadWrapper<DrawableBanner>
        {
            RelativeSizeAxes = Axes.Both,
            LoadContent = () => new DrawableBanner(user)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
        };

        avatar.Child = new LoadWrapper<DrawableAvatar>
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 8,
            Masking = true,
            LoadContent = () => new DrawableAvatar(user)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
        };

        topText.Clear();

        if (user.Club is not null)
        {
            topText.Add(new ClubTag(user.Club)
            {
                WebFontSize = 12,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Shadow = true
            });
        }

        if (user.NamePaint is not null)
        {
            topText.Add(new GradientText
            {
                Text = user.PreferredName,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 14,
                Colour = user.NamePaint.Colors.CreateColorInfo(),
                Shadow = true
            });
        }
        else
        {
            topText.Add(new FluXisSpriteText
            {
                Text = user.PreferredName,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                WebFontSize = 14,
                Shadow = true
            });
        }

        bottomText.FadeTo(user.HasDisplayName ? .8f : 0);
        bottomText.Text = user.Username;

        if (user.IsOnline)
        {
            statusIcon.Icon = FontAwesome6.Solid.Circle;
            statusIcon.Colour = Theme.Green;
            statusText.Text = "Online";
        }
        else
        {
            statusIcon.Icon = FontAwesome6.Regular.Circle;
            statusIcon.Colour = Theme.Foreground;
            statusText.Text = "Offline";
        }
    }
}
