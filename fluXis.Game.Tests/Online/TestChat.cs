using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
using fluXis.Shared.API.Packets.Chat;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Tests.Online;

public partial class TestChat : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(LoginOverlay login, FluxelClient fluxel)
    {
        Add(login);

        var flow = new FillFlowContainer
        {
            Direction = FillDirection.Vertical
        };

        Add(flow);

        fluxel.RegisterListener<ChatMessagePacket>(EventType.ChatMessage, response =>
        {
            Schedule(() =>
                flow.Add(new SpriteText
                {
                    Text = $"{response.Data.ChatMessage.Sender.Username} - {response.Data.Content}"
                }));
        });

        AddStep("Show login", login.Show);
        AddStep("Send message", () => fluxel.SendPacketAsync(ChatMessagePacket.CreateC2S("test", "general")));
    }
}
