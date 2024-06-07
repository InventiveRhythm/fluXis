using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.User;
using fluXis.Shared.Components.Users;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Online.Drawables;

public partial class DrawableUserCard : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay profile { get; set; }

    private APIUser user { get; }

    public DrawableUserCard(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(300, 80);
        CornerRadius = 20;
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
                        CornerRadius = 20,
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
