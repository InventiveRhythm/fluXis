using System;
using System.Text;
using Discord.Sdk;
using fluXis.Integration;
using fluXis.Online.API.Requests.Users.Connections;
using fluXis.Online.Fluxel;
using fluXis.Utils;
using JetBrains.Annotations;
using Midori.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Activity = Discord.Sdk.Activity;

namespace fluXis.Desktop.Integration;

public partial class DiscordManager : Drawable, IDiscordManager
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private FluXisGame game { get; set; }

    private const long client_id = 975141679583604767;

    [CanBeNull]
    public Action<long, string> OnInviteAccept;

    private Logger logger;
    private Client client;
    private string codeVerifier;
    private bool ready;

    private string token;

    [BackgroundDependencyLoader]
    private void load()
    {
        logger = Logger.GetLogger("DiscordSDK");
        logger.Add("SDK initializing.");

        client = new Client();
        client.AddLogCallback(onLog, LoggingSeverity.Verbose);
        client.SetStatusChangedCallback(onStatusChange);
        client.RegisterLaunchSteamApplication(client_id, SteamManager.APP_ID);
        client.SetActivityJoinCallback(onJoin);

        api.Activity.BindValueChanged(e =>
        {
            var activity = e.NewValue;

            if (activity == null) return;

            activity.API = api;
            updateRpc(activity.CreateDiscord());
        }, true);
    }

    protected override void Update()
    {
        base.Update();
        NativeMethods.Discord_RunCallbacks();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        api.Status.BindValueChanged(x =>
        {
            if (x.NewValue != ConnectionStatus.Online)
                return;

            if (!string.IsNullOrWhiteSpace(token))
                return;

            var req = new UserGetDiscordConnectionRequest(api.User.Value!.ID);
            req.Success += res =>
            {
                token = res.Data;
                loginWithToken(token);
            };
            req.Failure += _ => startAuthFlow();
            api.PerformRequestAsync(req);
        });
    }

    private void loginWithToken(string token) => client.UpdateToken(AuthorizationTokenType.Bearer, token, res =>
    {
        if (!res.Successful())
        {
            if (res.Retryable())
                Scheduler.AddDelayed(() => loginWithToken(token), 5000);

            return;
        }

        client.Connect();
    });

    private void onJoin(string secret)
    {
        var data = secret.Deserialize<LobbySecret>();
        OnInviteAccept?.Invoke(data.ID, data.Password);
    }

    private void onStatusChange(Client.Status status, Client.Error error, int errorDetail)
    {
        ready = status == Client.Status.Ready;
        logger.Add($"Status changed to {status.ToString()}.", LogLevel.Debug);
        if (error != Client.Error.None) logger.Add($"Error: {error.ToString()}, Code: {errorDetail}", LogLevel.Error);

        if (ready && api.Activity.Value != null)
        {
            var activity = api.Activity.Value;
            activity.API = api;
            updateRpc(activity.CreateDiscord());
        }
    }

    private void onLog(string message, LoggingSeverity severity) => logger.Add("SDK: " + message.Trim(), severity switch
    {
        LoggingSeverity.Verbose => LogLevel.Debug,
        LoggingSeverity.Info => LogLevel.Verbose,
        LoggingSeverity.Warning => LogLevel.Important,
        LoggingSeverity.Error => LogLevel.Error,
        LoggingSeverity.None => LogLevel.Verbose,
        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
    });

    #region RPC

    private void updateRpc(DiscordRichPresence rpc)
    {
        if (!ready)
            return;

        var activity = new Activity();
        activity.SetType((ActivityTypes)rpc.ActivityType);
        activity.SetStatusDisplayType((StatusDisplayTypes)rpc.DisplayType);
        ifNotEmpty(rpc.Details, 128, activity.SetDetails);
        ifNotEmpty(rpc.State, 128, activity.SetState);

        var assets = new ActivityAssets();
        ifNotEmpty(rpc.LargeImage, 300, assets.SetLargeImage);
        ifNotEmpty(rpc.LargeImageText, 128, assets.SetLargeText);
        ifNotEmpty(rpc.SmallImage, 300, assets.SetSmallImage);
        ifNotEmpty(rpc.SmallImageText, 128, assets.SetSmallText);
        activity.SetAssets(assets);

        var timestamps = new ActivityTimestamps();
        if (rpc.StartTime > 0) timestamps.SetStart(rpc.StartTime);
        if (rpc.EndTime > 0) timestamps.SetEnd(rpc.EndTime);
        activity.SetTimestamps(timestamps);

        if (rpc.PartyID > 0)
        {
            activity.SetSupportedPlatforms(ActivityGamePlatforms.Desktop);
            activity.SetApplicationId(client_id);

            var party = new ActivityParty();
            party.SetId($"multiplayer:{rpc.PartyID}");
            party.SetCurrentSize(rpc.PartySize);
            party.SetMaxSize(rpc.PartyMax);
            party.SetPrivacy(string.IsNullOrWhiteSpace(rpc.PartySecret) ? ActivityPartyPrivacy.Public : ActivityPartyPrivacy.Private);
            activity.SetParty(party);

            var secrets = new ActivitySecrets();
            secrets.SetJoin(new LobbySecret { ID = rpc.PartyID, Password = rpc.PartySecret }.Serialize());
            activity.SetSecrets(secrets);
        }

        client.UpdateRichPresence(activity, r =>
        {
            if (!r.Successful())
                logger.Add($"Failed to set RPC: {r.Error()}.", LogLevel.Error);
        });

        void ifNotEmpty([CanBeNull] string str, int maxBytes, Action<string> act)
        {
            if (!string.IsNullOrWhiteSpace(str))
                act(truncate(str, maxBytes));
        }

        string truncate(string value, int maxBytes)
        {
            if (string.IsNullOrEmpty(value))
                return value ?? "";

            var bytes = Encoding.UTF8.GetByteCount(value);

            if (bytes > maxBytes)
                return StringUtils.TruncateBytes(value, maxBytes);

            return value;
        }
    }

    #endregion

    #region Auth

    private void startAuthFlow()
    {
        var v = client.CreateAuthorizationCodeVerifier();
        codeVerifier = v.Verifier();

        var args = new AuthorizationArgs();
        args.SetClientId(client_id);
        args.SetScopes(Client.GetDefaultCommunicationScopes());
        args.SetCodeChallenge(v.Challenge());
        client.Authorize(args, onAuthorize);
    }

    private void onAuthorize(ClientResult result, string code, string redirectUri)
    {
        logger.Add($"Auth result: '{result.Error()}', code: {code}, redirect: {redirectUri}");

        if (!result.Successful())
            return;

        var req = new UserCreateDiscordConnectionRequest(api.User.Value.ID, code, redirectUri, codeVerifier);
        req.Success += res =>
        {
            token = res.Data;
            loginWithToken(res.Data);
        };
        api.PerformRequestAsync(req);
    }

    #endregion

    public class LobbySecret
    {
        [JsonProperty("id")]
        public long ID { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
