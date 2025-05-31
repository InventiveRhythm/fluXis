using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Requests.Chat;
using fluXis.Online.Chat;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Chat;

public partial class ChatChannelsPanel : Panel, ICloseable
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private ChatClient client { get; set; }

    public Action<string> OnJoinAction { get; init; }

    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(480, 520);

        Content.Child = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            ScrollbarVisible = false,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(12)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        StartLoading();

        var req = new ChatChannelsRequest();
        req.Success += addChannels;
        api.PerformRequestAsync(req);
    }

    private void addChannels(APIResponse<List<APIChatChannel>> res)
    {
        foreach (var channel in res.Data)
        {
            var alreadyAdded = client.Channels.Any(c => c.Name == channel.Name);

            // i'll make this look better sometime
            flow.Add(new ClickableContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 48,
                CornerRadius = 8,
                Masking = true,
                Enabled = { Value = !alreadyAdded },
                Alpha = alreadyAdded ? .6f : 1,
                Action = () => joinChannel(channel.Name),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FluXisSpriteText
                    {
                        Text = $"#{channel.Name}",
                        WebFontSize = 16,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        X = 16
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{channel.UserCount} users",
                        WebFontSize = 12,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        X = -16,
                        Alpha = .8f
                    }
                }
            });
        }

        StopLoading(false);
    }

    private void joinChannel(string name)
    {
        if (client.Channels.Any(c => c.Name == name))
            return;

        client.JoinChannel(name);
        OnJoinAction?.Invoke(name);
        Hide();
    }

    public void Close() => Hide();
}
