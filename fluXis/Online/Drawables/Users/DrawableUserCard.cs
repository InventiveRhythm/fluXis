using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Fluxel;
using fluXis.Overlay.User;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Online.Drawables.Users;

public partial class DrawableUserCard : CompositeDrawable, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>
            {
                new MenuActionItem("View Profile", FontAwesome6.Solid.User, MenuItemType.Highlighted, () => profile?.ShowUser(user.ID)),
                new MenuActionItem("Open in Web", FontAwesome6.Solid.EarthAmericas, MenuItemType.Normal, () => game?.OpenLink($"{api.Endpoint.WebsiteRootUrl}/u/{user.ID}")),
            };

            if (user.SteamID is not null)
                list.Add(new MenuActionItem("View Steam Profile", FontAwesome6.Brands.Steam, MenuItemType.Normal, () => game?.OpenLink(StringUtils.FormatSteamProfile(user.SteamID!.Value), true)));

            if (FluXisGameBase.IsDebug)
            {
                list.Add(new MenuSpacerItem());
                list.Add(new MenuActionItem("Copy ID", FontAwesome6.Solid.Copy, MenuItemType.Normal, () => clipboard.SetText($"{user.ID}")));
            }

            return list.ToArray();
        }
    }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay profile { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    public bool ShowLastOnline { get; set; } = true;

    private APIUser user { get; }

    public DrawableUserCard(APIUser user)
    {
        this.user = user;

        Width = 300;
        Height = 80;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        CornerRadius = 12;
        Masking = true;

        var badges = createBadges().ToArray();

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background4
            },
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
                Colour = FluXisColors.Background2,
                Alpha = 0.5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new LoadWrapper<DrawableAvatar>
                    {
                        Size = new Vector2(80),
                        CornerRadius = 12,
                        Masking = true,
                        LoadContent = () => new DrawableAvatar(user)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        OnComplete = avatar => avatar.FadeInFromZero(400)
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(2),
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.X,
                                Height = 16,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(4),
                                Children = new[]
                                {
                                    new ClubTag(user.Club)
                                    {
                                        WebFontSize = 12,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Shadow = true,
                                        Alpha = user.Club is null ? 0 : 1
                                    },
                                    createName().With(d =>
                                    {
                                        d.Anchor = Anchor.CentreLeft;
                                        d.Origin = Anchor.CentreLeft;
                                    })
                                }
                            },
                            new ForcedHeightText
                            {
                                Text = user.Username,
                                Height = 12,
                                Alpha = string.IsNullOrWhiteSpace(user.DisplayName) ? 0 : .8f,
                                WebFontSize = 12,
                                Shadow = true
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(5),
                                Alpha = badges.Length != 0 ? 1 : 0,
                                Children = badges
                            },
                            new ForcedHeightText
                            {
                                Text = $"Last online {TimeUtils.Ago(user.LastLogin ?? 0)}",
                                Height = 10,
                                Alpha = ShowLastOnline && !user.IsOnline && user.LastLogin != null ? .8f : 0,
                                WebFontSize = 10,
                                Shadow = true,
                                Margin = new MarginPadding { Top = 4 }
                            }
                        }
                    }
                }
            }
        };
    }

    private Drawable createName()
    {
        var paint = user.NamePaint;

        if (paint is not null)
        {
            return new GradientText
            {
                Colour = paint.Colors.CreateColorInfo(),
                Text = user.PreferredName,
                WebFontSize = 16,
                Shadow = true
            };
        }

        return new FluXisSpriteText
        {
            Text = user.PreferredName,
            WebFontSize = 16,
            Shadow = true
        };
    }

    private IEnumerable<Drawable> createBadges()
    {
        if (user.IsSupporter)
            yield return new DrawableSupporterBadge();

        foreach (var group in user.Groups)
            yield return new DrawableGroupBadge(group);
    }

    protected override bool OnClick(ClickEvent e)
    {
        profile?.ShowUser(user.ID);
        return true;
    }
}
