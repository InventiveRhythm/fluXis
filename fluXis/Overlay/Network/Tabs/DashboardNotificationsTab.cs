using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Models.Notifications;
using fluXis.Online.API.Models.Notifications.Data;
using fluXis.Online.Drawables.Clubs;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Fluxel;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardNotificationsTab : DashboardTab
{
    public override string Title => "Notifications";
    public override IconUsage Icon => FontAwesome6.Solid.Bell;
    public override DashboardTabType Type => DashboardTabType.Notifications;

    [Resolved]
    private IAPIClient api { get; set; }

    private double timeOpen;
    private bool updatedUnread;
    private FluXisScrollContainer scroll;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Child = scroll = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            ScrollbarOverlapsContent = true,
            ScrollbarAnchor = Anchor.TopLeft,
            ScrollbarOrigin = Anchor.TopLeft
        };
    }

    protected override void Update()
    {
        base.Update();

        timeOpen += Time.Elapsed;

        if (timeOpen > 2000 && !updatedUnread)
        {
            api.UpdateLastRead();
            updatedUnread = true;
        }
    }

    public override void Enter()
    {
        base.Enter();

        updatedUnread = false;
        timeOpen = 0;

        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(12)
        };

        foreach (var notification in ((IEnumerable<APINotification>)api.CurrentNotifications).Reverse())
            flow.Add(new DrawableNotification(notification, notification.Time > api.LastReadTime));

        scroll.Child = flow;
    }

    private partial class DrawableNotification : CompositeDrawable
    {
        [Resolved]
        private PanelContainer panels { get; set; }

        private APINotification notification { get; }
        private bool unread { get; }

        private HoverLayer hover;
        private FlashLayer flash;
        private Container icon;
        private FluXisSpriteText title;
        private FluXisTextFlow text;

        private Action action;

        public DrawableNotification(APINotification notification, bool unread)
        {
            this.notification = notification;
            this.unread = unread;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            CornerRadius = 8;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(12),
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ColumnDimensions = new Dimension[]
                        {
                            new(GridSizeMode.Absolute, 48),
                            new(GridSizeMode.Absolute, 12),
                            new()
                        },
                        RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) },
                        Content = new[]
                        {
                            new[]
                            {
                                new CircularContainer()
                                {
                                    Size = new Vector2(48),
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = FluXisColors.Background3
                                        },
                                        icon = new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                        }
                                    }
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Children = new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Children = new Drawable[]
                                            {
                                                title = new FluXisSpriteText
                                                {
                                                    WebFontSize = 20,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Text = TimeUtils.Ago(notification.Time),
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Alpha = .8f,
                                                    X = unread ? -16 : 0
                                                },
                                                new Circle
                                                {
                                                    Colour = FluXisColors.Highlight,
                                                    Size = new Vector2(8),
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Alpha = unread ? 1 : 0
                                                }
                                            }
                                        },
                                        text = new FluXisTextFlow
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            WebFontSize = 16
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            };

            switch (notification.Type)
            {
                case NotificationType.ClubInvite:
                {
                    var data = notification.GetDataAs<ClubInviteNotification>();

                    icon.Children = new Drawable[]
                    {
                        new LoadWrapper<DrawableClubIcon>
                        {
                            RelativeSizeAxes = Axes.Both,
                            LoadContent = () => new DrawableClubIcon(data.Club) { RelativeSizeAxes = Axes.Both },
                            OnComplete = i => i.FadeInFromZero(300)
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2,
                            Alpha = .5f
                        },
                        new FluXisSpriteIcon
                        {
                            Size = new Vector2(24),
                            Icon = FontAwesome6.Solid.CircleNodes,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Shadow = true
                        }
                    };

                    title.Text = "Club Invitation";

                    text.Text = $"You have been invited to join the '{data.Club.Name}' club!";
                    action = () => panels.Content = new ClubInvitePanel(data.InviteCode);
                    break;
                }
            }
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            action?.Invoke();
            flash.Show();
            return true;
        }
    }
}
