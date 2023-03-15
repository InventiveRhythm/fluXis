using fluXis.Game.Overlay.Notification;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooter : Container
{
    [Resolved]
    public NotificationOverlay Notifications { get; private set; }

    public SelectScreen Screen { get; }

    public SelectFooter(SelectScreen screen)
    {
        Screen = screen;

        RelativeSizeAxes = Axes.X;
        Height = 50;
        Anchor = Origin = Anchor.BottomLeft;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Children = new Drawable[]
                        {
                            new SelectFooterButton
                            {
                                Text = "Back",
                                Action = Screen.Exit
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = 100,
                                Name = "Spacer"
                            },
                            new SelectFooterButton
                            {
                                Text = "Mods",
                                Action = () => Notifications.Post("Work in progress...", "Come back later!")
                            },
                            new SelectFooterButton
                            {
                                Text = "Random",
                                Action = Screen.RandomMap
                            },
                            new SelectFooterButton
                            {
                                Text = "Options",
                                Action = () => Notifications.Post("Work in progress...", "Come back later")
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Children = new Drawable[]
                        {
                            new SelectFooterButton
                            {
                                Text = "Play",
                                Action = Screen.Accept
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e) => true; // Prevents the click from going through to the map list
}
