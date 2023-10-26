using System;
using DiscordRPC;
using fluXis.Game;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Desktop;

public class DiscordActivity
{
    private Fluxel fluxel;
    private DiscordRpcClient client;

    public void Initialize(Fluxel fluxel, Bindable<UserActivity> bind)
    {
        client = new DiscordRpcClient("975141679583604767");
        client.Initialize();

        this.fluxel = fluxel;

        bind.BindValueChanged(e =>
        {
            var activity = e.NewValue;

            if (activity == null) return;

            activity.Fluxel = fluxel;
            update(activity.Details, activity.Status, activity.Icon);
        });
    }

    private void update(string state = "", string details = "", string largeImageKey = "")
    {
        Assets assets = new Assets
        {
            LargeImageKey = largeImageKey,
            LargeImageText = $"fluXis {FluXisGameBase.VersionString}"
        };

        APIUserShort user = fluxel?.LoggedInUser;

        if (user != null)
        {
            assets.SmallImageKey = user.GetAvatarUrl(fluxel.Endpoint);
            assets.SmallImageText = user.Username;
        }

        client?.SetPresence(new RichPresence
        {
            Details = details.Substring(0, Math.Min(128, details.Length)),
            State = state.Substring(0, Math.Min(128, state.Length)),
            Assets = assets
        });

        Logger.Log($"Discord Rich Presence updated: {state} {details} {largeImageKey}", LoggingTarget.Network);
    }
}
