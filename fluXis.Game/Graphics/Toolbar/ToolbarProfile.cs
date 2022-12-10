using fluXis.Game.Online;
using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.Toolbar
{
    public class ToolbarProfile : Container
    {
        private Box hoverBox;
        private Container avatarBox;
        private DrawableAvatar avatar;
        private SpriteText usernameText;

        private const int hover_time = 200;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            Size = new Vector2(200, 60);
            Position = new Vector2(10, 0);
            CornerRadius = 10;
            Masking = true;

            APIUser user = UserCache.GetUser(1);

            InternalChildren = new Drawable[]
            {
                hoverBox = new Box
                {
                    Name = "Hover Box",
                    Colour = Colour4.White,
                    RelativeSizeAxes = Axes.Y,
                    Alpha = 0
                },
                avatarBox = new Container
                {
                    Size = new Vector2(50),
                    Position = new Vector2(5),
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        avatar = new DrawableAvatar(user)
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                usernameText = new SpriteText
                {
                    Text = user.Username,
                    Position = new Vector2(65, 0),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativePositionAxes = Axes.Y,
                    Font = new FontUsage("Quicksand", 32, "SemiBold")
                }
            };
        }

        protected override void Update()
        {
            float width = 5 + avatarBox.Width + 10 + usernameText.Width + 10;
            Size = new Vector2(width, Size.Y);
            hoverBox.Width = width;

            base.Update();
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeTo(.2f, hover_time);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeOut(hover_time);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            hoverBox.FadeTo(.4f).FadeTo(.2f, hover_time * 2);
            return base.OnClick(e);
        }
    }
}
