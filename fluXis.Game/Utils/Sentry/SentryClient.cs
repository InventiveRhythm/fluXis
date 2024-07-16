using System;
using System.Net;
using fluXis.Game.Configuration;
using fluXis.Game.Online.API;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Sentry;

namespace fluXis.Game.Utils.Sentry;

public partial class SentryClient : Component
{
    private FluXisGame game { get; }
    private IDisposable session { get; }
    private IBindable<APIUser> user { get; set; }

    public SentryClient(FluXisGame game)
    {
        this.game = game;

        Logger.Log("Initializing sentry client...");

        if (FluXisGameBase.IsDebug)
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

        Logger.NewEntry += onEntry;
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
        });
    }

    private void onEntry(LogEntry entry)
    {
        if (entry.Level != LogLevel.Error)
            return;

        var ex = entry.Exception;

        if (ex == null || shouldIgnore(ex))
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
        });
    }

    private bool shouldIgnore(Exception ex)
    {
        switch (ex)
        {
            case APIException:
                return true;

            case WebException web:
                return web.Status == WebExceptionStatus.Timeout;
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
