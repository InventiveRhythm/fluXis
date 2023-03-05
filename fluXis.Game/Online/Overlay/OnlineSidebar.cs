using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Overlay.Sidebar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Online.Overlay;

public partial class OnlineSidebar : Container
{
    private DrawableAvatar avatar;
    private SidebarBanner banner;
    private UsernameBox usernameBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Width = .2f;
        CornerRadius = 10;
        Masking = true;

        Box usernameBackground = new Box
        {
            RelativeSizeAxes = Axes.X,
            Colour = Colour4.White,
            Alpha = 0,
            Height = 60,
            Y = 190
        };

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new BasicScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    usernameBackground,
                    new Container
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 200,
                        Masking = true,
                        CornerRadius = 10,
                        Children = new Drawable[]
                        {
                            banner = new SidebarBanner(null)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            new Container
                            {
                                Size = new Vector2(150),
                                Masking = true,
                                CornerRadius = 10,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Child = avatar = new DrawableAvatar(null)
                                {
                                    RelativeSizeAxes = Axes.Both
                                }
                            }
                        }
                    },
                    usernameBox = new UsernameBox(usernameBackground, null)
                }
            }
        };
    }

    public void OnUserLogin()
    {
        APIUser user = Fluxel.Fluxel.GetLoggedInUser();
        avatar.UpdateUser(user);
        banner.UpdateUser(user);
        usernameBox.UpdateUser(user);
    }

    protected partial class UsernameBox : Container
    {
        private readonly APIUser user;
        private readonly Box background;
        private SpriteText username;

        public UsernameBox(Box background, APIUser user)
        {
            this.background = background;
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Y = 200;
            Height = 50;
            RelativeSizeAxes = Axes.X;

            Child = username = new SpriteText
            {
                Name = "Username",
                Text = user?.Username ?? "Not logged in",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = new FontUsage("Quicksand", 30, "SemiBold")
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeTo(.2f, 200);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeTo(0, 200);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            background.FadeTo(.4f).FadeTo(.2f, 200);
            Logger.Log("Clicked on username... (not implemented yet)");
            return true;
        }

        public void UpdateUser(APIUserShort user)
        {
            username.Text = user?.Username ?? "Not logged in";
        }
    }
}