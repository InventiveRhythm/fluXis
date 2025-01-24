using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using fluXis.Configuration;
using fluXis.Online.API;
using fluXis.Online.API.Models.Users;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Sentry;

namespace fluXis.Utils.Sentry;

public partial class SentryClient : Component
{
    private FluXisGame game { get; }
    private IDisposable session { get; }
    private IBindable<APIUser> user { get; set; }

    public SentryClient(FluXisGame game)
    {
        this.game = game;

        Logger.Log("Initializing sentry client...");

        Logger.NewEntry += onEntry;

        if (FluXisGameBase.IsDebug || !RuntimeInfo.IsDesktop)
            return;

        session = SentrySdk.Init(opt =>
        {
            opt.Dsn = "https://a297495ce509e0643fb019fa5a264921@sentry.flux.moe/2";
            opt.IsEnvironmentUser = false;
            opt.AutoSessionTracking = true;
            opt.IsGlobalModeEnabled = true;
            opt.Release = FluXisGameBase.VersionString.TrimStart('v');
        });

        Logger.Log("Initialized sentry client.");
    }

    public void BindUser(IBindable<APIUser> bind)
    {
        if (session == null)
            return;

        user = bind.GetBoundCopy();
        user.BindValueChanged(e =>
        {
            SentrySdk.ConfigureScope(s => s.User = new SentryUser
            {
                Username = e.NewValue?.Username ?? "Guest User",
                Id = $"{e.NewValue?.ID ?? 0}"
            });
        }, true);
    }

    private void onEntry(LogEntry entry)
    {
        if (entry.Level != LogLevel.Error)
            return;

        var ex = entry.Exception;

        if (ex == null)
            return;

        if (shouldIgnore(ex))
        {
            Logger.Log($"Ignored {ex.GetType().Name}!", LoggingTarget.Runtime, LogLevel.Debug);
            return;
        }

        Logger.Log($"{ex.GetType().Name} would be reported!", LoggingTarget.Runtime, LogLevel.Debug);

        if (session is null)
            return;

        SentrySdk.CaptureEvent(new SentryEvent(ex)
        {
            Message = entry.Message,
            Level = entry.Level switch
            {
                LogLevel.Debug => SentryLevel.Debug,
                LogLevel.Verbose => SentryLevel.Info,
                LogLevel.Important => SentryLevel.Warning,
                LogLevel.Error => SentryLevel.Error,
                _ => throw new ArgumentOutOfRangeException()
            }
        }, scope =>
        {
            scope.Contexts["config"] = new
            {
                Game = game.Dependencies.Get<FluXisConfig>()?.GetCurrentConfigurationForLogging(),
                Framework = game.Dependencies.Get<FrameworkConfigManager>()?.GetCurrentConfigurationForLogging(),
            };

            scope.Contexts["hashes"] = new
            {
                Game = game.ClientHash,
                plugins = game.Plugins?.Plugins.ToDictionary(x => x.Name, x => x.Hash)
            };
        });
    }

    private bool shouldIgnore(Exception ex)
    {
        switch (ex)
        {
            case APIException:
                return true;

            case WebException web:
                if (web.Response is HttpWebResponse { StatusCode: System.Net.HttpStatusCode.BadGateway or System.Net.HttpStatusCode.ServiceUnavailable })
                    return true;

                return web.Status is WebExceptionStatus.Timeout or WebExceptionStatus.UnknownError;

            case WebSocketException ws:
                return ws.WebSocketErrorCode == WebSocketError.NotAWebSocket;
        }

        return false;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Logger.NewEntry -= onEntry;
        session?.Dispose();
    }
}
