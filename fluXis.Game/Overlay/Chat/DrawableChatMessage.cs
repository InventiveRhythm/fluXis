using System;
using fluXis.Game.Graphics;
using fluXis.Game.Online.Chat;
using fluXis.Game.Overlay.Profile;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Chat;

public partial class DrawableChatMessage : Container
{
    [Resolved]
    private ProfileOverlay profile { get; set; }

    public ChatMessage Message { get; set; }

    private FillFlowContainer flow;

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
                    profile.UpdateUser(Message.Sender.ID);
                }
            },
            flow = new FillFlowContainer
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
                            new SpriteText
                            {
                                Text = Message.Sender.Username,
                                Font = FluXisFont.Default(22)
                            },
                            new SpriteText
                            {
                                Text = DateTimeOffset.FromUnixTimeMilliseconds(Message.Timestamp).ToString("HH:mm"),
                                Font = FluXisFont.Default(18),
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };

        LoadComponentAsync(new DrawableAvatar(Message.Sender) { RelativeSizeAxes = Axes.Both }, avatar =>
        {
            avatarContainer.Add(avatar);
            avatar.FadeInFromZero(200);
        });
        AddMessage(Message);
    }

    public void AddMessage(ChatMessage message)
    {
        flow.Add(new SpriteText
        {
            Text = message.Content,
            Font = FluXisFont.Default(18),
            Margin = new MarginPadding { Bottom = 5 }
        });
    }
}
