using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.User;
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

namespace fluXis.Online.Drawables;

public partial class DrawableUserCard : CompositeDrawable, IHasContextMenu
{
    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>
            {
                new FluXisMenuItem("View Profile", FontAwesome6.Solid.User, MenuItemType.Highlighted, () => profile?.ShowUser(user.ID)),
                new FluXisMenuItem("Open in Web", FontAwesome6.Solid.EarthAmericas, MenuItemType.Normal, () => game?.OpenLink($"{api.Endpoint.WebsiteRootUrl}/u/{user.ID}")),
            };

            if (FluXisGameBase.IsDebug)
            {
                list.Add(new FluXisMenuSpacer());
                list.Add(new FluXisMenuItem("Copy ID", FontAwesome6.Solid.Copy, MenuItemType.Normal, () => clipboard.SetText($"{user.ID}")));
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
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = user.PreferredName,
                                WebFontSize = 20,
                                Shadow = true
                            },
                            new FluXisSpriteText
                            {
                                Text = user.Username,
                                Alpha = string.IsNullOrWhiteSpace(user.DisplayName) ? 0 : .8f,
                                Margin = new MarginPadding { Top = -5 },
                                WebFontSize = 12,
                                Shadow = true
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(5),
                                Alpha = user.Groups.Count != 0 ? 1 : 0,
                                ChildrenEnumerable = user.Groups.Select(group => new DrawableGroupBadge(group))
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        profile?.ShowUser(user.ID);
        return true;
    }
}
