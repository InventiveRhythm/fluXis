using System;
using System.Net.WebSockets;
using System.Threading;
using osu.Framework.Logging;

namespace fluXis.Game.Online
{
    public class Fluxel
    {
        private static ClientWebSocket connection;

        public static async void Connect()
        {
            Logger.Log("Connecting to server...");
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri("wss://fluxel.foxes4life.net"), CancellationToken.None);

            // create thread
            Thread thread = new Thread(receive);
            thread.Start();
            Logger.Log("Connected to server.");

            Logger.Log("Sending message...");
            Send("{\"id\":-1}");
        }

        private static async void receive()
        {
            while (connection.State == WebSocketState.Open)
            {
                // receive data
                byte[] buffer = new byte[2048];
                await connection.ReceiveAsync(buffer, CancellationToken.None);

                // convert to string
                string message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                Logger.Log(message);
            }

            Logger.Log("Disconnected from server.");
        }

        public static void Send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            connection.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
