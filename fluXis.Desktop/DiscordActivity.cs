using System;
using System.Linq;
using DiscordRPC;
using DiscordRPC.Message;
using fluXis.Game;
using fluXis.Game.Integration;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Utils;
using Newtonsoft.Json;
using osu.Framework.Logging;
using EventType = DiscordRPC.EventType;

namespace fluXis.Desktop;

public class DiscordActivity
{
    private FluXisGame game;
    private DiscordRpcClient client;

    public void Initialize(FluXisGame game, IAPIClient api)
    {
        this.game = game;

        var secondPipe = Program.Args.Contains("--rpc-pipe-2");

        client = new DiscordRpcClient("975141679583604767", pipe: secondPipe ? 1 : -1);

        client.RegisterUriScheme();
        client.Subscribe(EventType.Join);
        client.OnJoin += onJoin;

        client.OnConnectionEstablished += (_, args) => Logger.Log($"RPC is on pipe {args.ConnectedPipe}", LoggingTarget.Network, LogLevel.Debug);
        client.OnError += (_, args) => Logger.Log($"Discord RPC error: {args.Message}.", LoggingTarget.Network, LogLevel.Error);
        var success = client.Initialize();

        if (!success)
        {
            Logger.Log("Discord Rich Presence failed to initialize!", LoggingTarget.Network, LogLevel.Error);
            return;
        }

        api.Activity.BindValueChanged(e =>
        {
            var activity = e.NewValue;

            if (activity == null) return;

            activity.API = api;
            client?.SetPresence(build(activity.CreateDiscord()));
        }, true);
    }

    private void onJoin(object sender, JoinMessage args)
    {
        var secret = args.Secret.Deserialize<LobbySecret>();
        game.JoinMultiplayerRoom(secret.ID, secret.Password);
    }

    private static RichPresence build(DiscordRichPresence rpc)
    {
        var discord = new RichPresence
        {
            State = rpc.State[..Math.Min(rpc.State.Length, 128)],
            Details = rpc.Details[..Math.Min(rpc.Details.Length, 128)],
            Assets = new Assets
            {
                LargeImageKey = rpc.LargeImage,
                LargeImageText = rpc.LargeImageText,
                SmallImageKey = rpc.SmallImage,
                SmallImageText = rpc.SmallImageText
            },
            Timestamps = new Timestamps()
        };

        if (rpc.StartTime > 0)
            discord.Timestamps.StartUnixMilliseconds = rpc.StartTime;
        if (rpc.EndTime > 0)
            discord.Timestamps.EndUnixMilliseconds = rpc.EndTime;

        if (rpc.PartyID > 0)
        {
            discord.Party = new Party
            {
                Privacy = Party.PrivacySetting.Public,
                ID = $"multiplayer:{rpc.PartyID}",
                Size = rpc.PartySize,
                Max = rpc.PartyMax
            };

            discord.Secrets = new Secrets
            {
                JoinSecret = new LobbySecret
                {
                    ID = rpc.PartyID,
                    Password = rpc.PartySecret
                }.Serialize()
            };
        }
        else if (rpc.Buttons is not null)
            discord.Buttons = rpc.Buttons.Select(b => new Button { Label = b.Label, Url = b.Url }).ToArray();

        Logger.Log($"Setting Discord RPC to: {discord.Serialize()}", LoggingTarget.Network, LogLevel.Debug);
        return discord;
    }

    public class LobbySecret
    {
        [JsonProperty("id")]
        public long ID { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
