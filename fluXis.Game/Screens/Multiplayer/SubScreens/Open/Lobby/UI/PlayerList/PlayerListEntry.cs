using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby.UI.PlayerList;

public partial class PlayerListEntry : Container
{
    public APIUserShort User { get; set; }

    private SpriteIcon readyIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Masking = true;
        CornerRadius = 10;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            new DrawableAvatar(User)
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Size = new Vector2(50)
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 50 },
                Children = new Drawable[]
                {
                    new DrawableBanner(User)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.3f
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding { Left = 10 },
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = User.Username,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                FontSize = 30
                            }
                        }
                    },
                    readyIcon = new SpriteIcon
                    {
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Icon = FontAwesome6.Solid.Check,
                        Size = new Vector2(20),
                        Shadow = true,
                        Margin = new MarginPadding { Right = 10 },
                        Alpha = 0,
                        Colour = Colour4.FromHex("#7BE87B")
                    }
                }
            }
        };
    }

    public void SetReady(bool ready)
    {
        readyIcon.Alpha = ready ? 1 : 0;
    }
}
