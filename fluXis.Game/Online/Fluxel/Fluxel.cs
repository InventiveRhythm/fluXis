using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel
{
    public class Fluxel
    {
        internal static HttpClient Http = new();
        private static ClientWebSocket connection;
        private static APIUser loggedInUser;

        public static string Token;

        private static readonly ConcurrentDictionary<EventType, List<Action<object>>> response_listeners = new();

        public static async void Connect()
        {
            Logger.Log("Connecting to server...");
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(APIConstants.WEBSOCKET_URL), CancellationToken.None);

            // create thread
            receive();
            Logger.Log("Connected to server.");
        }

        private static async void receive()
        {
            try
            {
                while (connection.State == WebSocketState.Open)
                {
                    // receive data
                    byte[] buffer = new byte[2048];
                    await connection.ReceiveAsync(buffer, CancellationToken.None);

                    // convert to string
                    string message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                    // handler logic
                    void handleListener<T>()
                    {
                        var response = FluxelResponse<T>.Parse(message);

                        if (response_listeners.ContainsKey(response.Type))
                        {
                            foreach (var listener
                                     in (IEnumerable<Action<object>>)response_listeners.GetValueOrDefault(response.Type)
                                        ?? ArraySegment<Action<object>>.Empty)
                            {
                                listener(response);
                            }
                        }
                    }

                    // find right handler
                    Action handler = (EventType)JsonConvert.DeserializeObject<JObject>(message)["id"]!.ToObject<int>() switch
                    {
                        EventType.Token => handleListener<string>,
                        EventType.Login => handleListener<APIUser>,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    // execute handler
                    handler();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Fatal Error in receiver: " + e);
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

        public static void RegisterListener<T>(EventType id, Action<FluxelResponse<T>> listener)
        {
            response_listeners.GetOrAdd(id, _ => new()).Add(response => listener((FluxelResponse<T>)response));
        }

        public static void UnregisterListener(EventType id)
        {
            response_listeners.Remove(id, out var listeners);
            listeners?.Clear();
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

    public enum EventType : int
    {
        Token = 0,
        Login = 1
    }
}
