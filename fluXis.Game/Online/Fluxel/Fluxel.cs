using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel.Packets;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel
{
    public class Fluxel
    {
        private static ClientWebSocket connection;
        private static APIUser loggedInUser;

        private static readonly Dictionary<int, IResponseListener> response_listeners = new Dictionary<int, IResponseListener>();

        public static async void Connect()
        {
            Logger.Log("Connecting to server...");
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(APIConstants.WEBSOCKET_URL), CancellationToken.None);

            // create thread
            Thread thread = new Thread(receive);
            thread.Start();
            Logger.Log("Connected to server.");
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
                FluxelResponse<dynamic> response = JsonConvert.DeserializeObject<FluxelResponse<dynamic>>(message);

                if (response_listeners.ContainsKey(response.ID))
                {
                    response_listeners[response.ID].Invoke(message);
                }
            }

            Logger.Log("Disconnected from server.");
        }

        public static void Send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            connection.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static void SendPacket(Packet packet)
        {
            FluxelRequest request = new FluxelRequest(packet.ID, packet);
            string json = JsonConvert.SerializeObject(request);
            Send(json);
        }

        public static void RegisterListener(int id, IResponseListener listener)
        {
            response_listeners.Add(id, listener);
        }

        public static void UnregisterListener(int id)
        {
            response_listeners.Remove(id);
        }

        public static void SetLoggedInUser(APIUser user)
        {
            loggedInUser = user;
            Logger.Log($"Logged in as {user.Username}");
        }

        public static APIUser GetLoggedInUser()
        {
            return loggedInUser;
        }
    }
}
