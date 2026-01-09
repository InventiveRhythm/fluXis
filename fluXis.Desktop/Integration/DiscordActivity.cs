using System;
using System.Linq;
using System.Text;
using DiscordRPC;
using DiscordRPC.Message;
using fluXis.Integration;
using fluXis.Online.Fluxel;
using fluXis.Utils;
using Humanizer;
using Newtonsoft.Json;
using osu.Framework.Logging;
using EventType = DiscordRPC.EventType;

namespace fluXis.Desktop.Integration;

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

    private static string truncateWithLog(string value, int maxBytes, string fieldName = null)
    {
        string field = !string.IsNullOrEmpty(fieldName) ? $"{fieldName}" : "";

        if (string.IsNullOrEmpty(value))
        {
            Logger.Log($"Discord RPC {field} is empty or null.", LoggingTarget.Network, LogLevel.Verbose);
            return value ?? "";
        }
        
        var bytes = Encoding.UTF8.GetByteCount(value);
        
        if (bytes > maxBytes)
        {
            Logger.Log($"Discord RPC {field} exceeded {maxBytes} bytes (was {bytes} bytes), truncating...", LoggingTarget.Network, LogLevel.Verbose);
            return StringUtils.TruncateBytes(value, maxBytes);
        }
        
        return value;
    }

    private static RichPresence build(DiscordRichPresence rpc)
    {
        var discord = new RichPresence
        {
            State = truncateWithLog(rpc.State, 128, "State"),
            Details = truncateWithLog(rpc.Details, 128, "Details"),
            Assets = new Assets
            {
                LargeImageKey = truncateWithLog(rpc.LargeImage, 300, "LargeImageKey"),
                LargeImageText = truncateWithLog(rpc.LargeImageText, 128, "LargeImageText"),
                SmallImageKey = truncateWithLog(rpc.SmallImage, 300, "SmallImageKey"),
                SmallImageText = truncateWithLog(rpc.SmallImageText, 128, "SmallImageText")
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
