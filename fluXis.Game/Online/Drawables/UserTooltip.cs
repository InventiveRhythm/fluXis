using System;
using System.Threading.Tasks;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Online.Drawables;

public partial class UserTooltip : Container
{
    public long UserID { get; set; }

    private FluXisSpriteText username;
    private FluXisSpriteText onlineStatus;
    private FluXisSpriteText lastOnline;
    private DrawableAvatar avatar;
    private DrawableBanner banner;
    private Container loadingContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 80;
        Width = 300;
        CornerRadius = 20;

        InternalChildren = new Drawable[]
        {
            banner = new DrawableBanner(null)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.5f),
                Width = .4f
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Colour = ColourInfo.GradientHorizontal(Colour4.Black.Opacity(.5f), Colour4.Black.Opacity(.2f)),
                Width = .6f,
                X = .4f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(60),
                        CornerRadius = 10,
                        Masking = true,
                        Child = avatar = new DrawableAvatar(null)
                        {
                            RelativeSizeAxes = Axes.Both,
                            FillMode = FillMode.Fill,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Left = 70 },
                        Children = new Drawable[]
                        {
                            username = new FluXisSpriteText
                            {
                                Shadow = true,
                                FontSize = 28
                            },
                            onlineStatus = new FluXisSpriteText
                            {
                                Margin = new MarginPadding { Top = 24 },
                                Shadow = true,
                                FontSize = 18
                            },
                            lastOnline = new FluXisSpriteText
                            {
                                Margin = new MarginPadding { Top = 40 },
                                Shadow = true,
                                FontSize = 15
                            }
                        }
                    }
                }
            },
            loadingContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background4
                    },
                    new LoadingIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(30)
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Task.Run(loadUser);
    }

    private void loadUser()
    {
        var user = UserCache.GetUser(UserID);

        avatar.UpdateUser(user);
        banner.UpdateUser(user);
        username.Text = user.Username;
        onlineStatus.Text = user.IsOnline ? "Online" : "Offline";
        lastOnline.Text = user.IsOnline ? "Right Now" : $"Last seen {TimeUtils.Ago(DateTimeOffset.FromUnixTimeSeconds(user.LastLogin))}";

        Schedule(() => loadingContainer.FadeOut(200));
    }
}
