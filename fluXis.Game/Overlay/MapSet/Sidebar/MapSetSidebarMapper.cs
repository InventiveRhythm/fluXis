using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.Drawables;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Sidebar;

public partial class MapSetSidebarMapper : CompositeDrawable
{
    [Resolved]
    [CanBeNull]
    private FluXisGame game { get; set; }

    private Bindable<APIMap> mapBind { get; }

    private long id;

    private LoadWrapper<DrawableBanner> banner;
    private LoadWrapper<DrawableAvatar> avatar;
    private FluXisSpriteText name;

    public MapSetSidebarMapper(Bindable<APIMap> mapBind)
    {
        this.mapBind = mapBind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;
        CornerRadius = 8;
        Masking = true;

        id = mapBind.Value.Mapper.ID;

        InternalChildren = new Drawable[]
        {
            banner = new LoadWrapper<DrawableBanner>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableBanner(mapBind.Value.Mapper)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = b => b.FadeIn(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8),
                Children = new Drawable[]
                {
                    avatar = new LoadWrapper<DrawableAvatar>
                    {
                        Size = new Vector2(48),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        CornerRadius = 8,
                        Masking = true,
                        LoadContent = () => new DrawableAvatar(mapBind.Value.Mapper)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        OnComplete = a => a.FadeIn(400)
                    },
                    name = new FluXisSpriteText
                    {
                        Text = mapBind.Value.Mapper.PreferredName,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 14,
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        mapBind.ValueChanged += m =>
        {
            if (id == m.NewValue.Mapper.ID)
                return;

            banner.Reload();
            avatar.Reload();
            name.Text = m.NewValue.Mapper.PreferredName;
            id = mapBind.Value.Mapper.ID;
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        game?.PresentUser(mapBind.Value.Mapper.ID);
        return true;
    }
}
