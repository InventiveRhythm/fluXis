using System;
using DiscordRPC;
using fluXis.Game;
using fluXis.Game.Activity;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;

namespace fluXis.Desktop;

public class DiscordActivity : IActivity
{
    private static DiscordRpcClient client;

    public void Initialize()
    {
        client = new DiscordRpcClient("975141679583604767");
        client.Initialize();
    }

    public void OnRemove() => client?.Dispose();

    public void Update(Fluxel fluxel, string details = "", string state = "", string largeImageKey = "", int timestamp = 0, int timeLeft = 0)
    {
        Timestamps timestamps = new Timestamps();

        if (timestamp != 0)
            timestamps.Start = DateTime.UtcNow.AddSeconds(timestamp);
        else if (timeLeft != 0)
            timestamps.End = DateTime.UtcNow.AddSeconds(timeLeft);

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
            Details = details,
            State = state,
            Timestamps = timestamps,
            Assets = assets
        });
    }
}
