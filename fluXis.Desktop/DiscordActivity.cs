using System;
using DiscordRPC;
using fluXis.Game;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Desktop;

public class DiscordActivity
{
    private FluxelClient fluxel;
    private DiscordRpcClient client;

    public void Initialize(FluxelClient fluxel, Bindable<UserActivity> bind)
    {
        client = new DiscordRpcClient("975141679583604767");
        var success = client.Initialize();

        if (!success)
        {
            Logger.Log("Discord Rich Presence failed to initialize!", LoggingTarget.Network, LogLevel.Error);
            return;
        }

        this.fluxel = fluxel;

        bind.BindValueChanged(e =>
        {
            var activity = e.NewValue;

            if (activity == null) return;

            activity.Fluxel = fluxel;
            update(activity.Details, activity.Status, activity.Icon);
        }, true);
    }

    private void update(string state = "", string details = "", string largeImageKey = "")
    {
        Assets assets = new Assets
        {
            LargeImageKey = largeImageKey,
            LargeImageText = $"fluXis {FluXisGameBase.VersionString}"
        };

        var user = fluxel?.User.Value;

        if (user != null)
        {
            assets.SmallImageKey = $"{fluxel.Endpoint.AssetUrl}/avatar/{user.ID}";
            assets.SmallImageText = user.Username;
        }

        client?.SetPresence(new RichPresence
        {
            Details = details.Substring(0, Math.Min(128, details.Length)),
            State = state.Substring(0, Math.Min(128, state.Length)),
            Assets = assets
        });

        Logger.Log($"Discord Rich Presence updated: {getOrEmpty(state)}, {getOrEmpty(details)}, {getOrEmpty(largeImageKey)}!", LoggingTarget.Network, LogLevel.Debug);
    }

    private string getOrEmpty(string str) => string.IsNullOrEmpty(str) ? "<empty>" : str;
}
