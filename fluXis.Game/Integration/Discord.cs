using System;
using DiscordRPC;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;

namespace fluXis.Game.Integration;

public class Discord
{
    private static DiscordRpcClient client;

    public static void Init()
    {
        client = new DiscordRpcClient("975141679583604767");
        client.Initialize();
        client.OnReady += (_, _) => Update("In the menus", "Idle", "menu");
    }

    public static void Update(string details = "", string state = "", string largeImageKey = "", int timestamp = 0, int timeLeft = 0)
    {
        Timestamps timestamps = new Timestamps();

        if (timestamp != 0)
            timestamps.Start = DateTime.UtcNow.AddSeconds(timestamp);
        else if (timeLeft != 0)
            timestamps.End = DateTime.UtcNow.AddSeconds(timeLeft);

        Assets assets = new Assets
        {
            LargeImageKey = largeImageKey,
            LargeImageText = "fluXis"
        };

        APIUser user = Fluxel.GetLoggedInUser();

        if (user != null)
        {
            assets.SmallImageKey = "https://api.fluxis.foxes4life.net/assets/avatar/" + user.ID;
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

    public static void Reload()
    {
        // TODO: Calculate times

        Update(client?.CurrentPresence?.Details, client?.CurrentPresence?.State, client?.CurrentPresence?.Assets?.LargeImageKey);
    }
}
