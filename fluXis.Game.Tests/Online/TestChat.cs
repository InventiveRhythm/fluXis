using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Chat;
using fluXis.Game.Overlay.Login;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Tests.Online;

public partial class TestChat : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(LoginOverlay login, Fluxel fluxel)
    {
        Add(login);

        var flow = new FillFlowContainer
        {
            Direction = FillDirection.Vertical
        };

        Add(flow);

        fluxel.RegisterListener<ChatMessage>(EventType.ChatMessage, response =>
        {
            Schedule(() =>
                flow.Add(new SpriteText
                {
                    Text = $"{response.Data.Sender.Username} - {response.Data.Content}"
                }));
        });

        AddStep("Show login", login.Show);
        AddStep("Send message", () => fluxel.SendPacketAsync(new ChatMessagePacket
        {
            Channel = "general",
            Content = "test"
        }));
    }
}
