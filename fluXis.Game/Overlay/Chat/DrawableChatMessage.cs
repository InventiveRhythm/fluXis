using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menu;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Chat;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Profile;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Overlay.Chat;

public partial class DrawableChatMessage : Container
{
    [Resolved]
    private ProfileOverlay profile { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    public ChatMessage InitialMessage { get; set; }
    public List<ChatMessage> Messages { get; } = new();

    private FillFlowContainer<MessageText> flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        ClickableContainer avatarContainer;

        InternalChildren = new Drawable[]
        {
            avatarContainer = new ClickableContainer
            {
                Size = new Vector2(40),
                Masking = true,
                CornerRadius = 5,
                Action = () =>
                {
                    profile.Show();
                    profile.UpdateUser(InitialMessage.Sender.ID);
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Left = 50 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = InitialMessage.Sender.Username,
                                FontSize = 22,
                                Colour = InitialMessage.Sender.Role == 0 ? FluXisColors.Text : FluXisColors.GetRoleColor(InitialMessage.Sender.Role)
                            },
                            new FluXisSpriteText
                            {
                                Text = DateTimeOffset.FromUnixTimeSeconds(InitialMessage.Timestamp).ToString("HH:mm"),
                                FontSize = 18,
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    },
                    flow = new FillFlowContainer<MessageText>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(5)
                    }
                }
            }
        };

        LoadComponentAsync(new DrawableAvatar(InitialMessage.Sender)
        {
            RelativeSizeAxes = Axes.Both,
            ShowTooltip = true
        }, avatar =>
        {
            avatarContainer.Add(avatar);
            avatar.FadeInFromZero(200);
        });

        AddMessage(InitialMessage);
    }

    public void AddMessage(ChatMessage message)
    {
        Messages.Add(message);
        flow.Add(new MessageText
        {
            Message = message,
            Fluxel = fluxel
        });
    }

    public void RemoveMessage(string id)
    {
        flow.Remove(flow.FirstOrDefault(m => m.Message.Id == id), true);
        Messages.Remove(Messages.FirstOrDefault(m => m.Id == id));
    }

    private partial class MessageText : FluXisTextFlow, IHasContextMenu
    {
        public ChatMessage Message { get; init; }
        public Fluxel Fluxel { get; init; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            FontSize = 18;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            AddText(Message.Content);
        }

        public MenuItem[] ContextMenuItems
        {
            get
            {
                List<MenuItem> items = new List<MenuItem>
                {
                    Fluxel.LoggedInUser?.Role >= 3
                        ? new FluXisMenuItem("Delete", MenuItemType.Dangerous, delete)
                        : new FluXisMenuItem("Report", MenuItemType.Dangerous, report)
                };

                return items.ToArray();
            }
        }

        private async void delete()
        {
            await Fluxel.SendPacket(new ChatMessageDeletePacket { Id = Message.Id });
        }

        private /* async*/ void report()
        {
            notifications.PostError("This feature is not yet implemented.");
            // await Fluxel.SendPacket(new ChatMessageReportPacket { Id = Message.Id });
        }
    }
}
