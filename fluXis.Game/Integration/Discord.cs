using System;
using DiscordRPC;

namespace fluXis.Game.Integration
{
    public class Discord
    {
        private static DiscordRpcClient client;

        public static void Init()
        {
            client = new DiscordRpcClient("975141679583604767");
            client.Initialize();
            client.OnReady += (sender, e) => Update("In the menus", "Idle");
        }

        public static void Update(string details = "", string state = "", string largeImageKey = "", int timestamp = 0, int timeLeft = 0)
        {
            Timestamps timestamps = new Timestamps();

            if (timestamp != 0)
                timestamps.Start = DateTime.UtcNow.AddSeconds(timestamp);
            else if (timeLeft != 0)
                timestamps.End = DateTime.UtcNow.AddSeconds(timeLeft);

            client?.SetPresence(new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = timestamps,
                Assets = new Assets
                {
                    LargeImageKey = largeImageKey,
                    LargeImageText = "fluXis",
                    SmallImageKey = "https://api.fluxis.foxes4life.net/assets/avatar/1",
                    SmallImageText = "Flustix"
                }
            });
        }
    }
}
