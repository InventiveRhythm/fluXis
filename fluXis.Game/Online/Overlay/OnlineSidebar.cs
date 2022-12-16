using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Overlay.Sidebar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Online.Overlay
{
    public class OnlineSidebar : Container
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            Width = 300;
            CornerRadius = 10;
            Masking = true;

            APIUser user = UserCache.GetUser(1);

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#222228")
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 200,
                    Children = new Drawable[]
                    {
                        new SidebarBanner(user)
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        new Container
                        {
                            Size = new Vector2(150),
                            Masking = true,
                            CornerRadius = 5,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Child = new DrawableAvatar(user)
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    }
                }
            };
        }
    }
}
