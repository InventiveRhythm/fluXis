using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel.Packets;
using fluXis.Game.Overlay.Notification;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel;

public class Fluxel
{
    internal static HttpClient Http = new();
    private static ClientWebSocket connection;
    private static APIUser loggedInUser;

    private static readonly List<string> packet_queue = new();

    public static string Token;
    public static Action<APIUser> OnUserLoggedIn;

    public static NotificationOverlay Notifications;

    private static readonly ConcurrentDictionary<EventType, List<Action<object>>> response_listeners = new();

    public static async void Connect()
    {
        try
        {
            Notifications.Post("Connecting to server...");
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(APIConstants.WebsocketUrl), CancellationToken.None);

            // create thread
            receive();
            Notifications.Post("Connected to server!");

            // send queued packets
            foreach (var packet in packet_queue)
                Send(packet);
        }
        catch (Exception ex)
        {
            Notifications.Post("Failed to connect to server!");
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);
        }
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
                Logger.Log(message, LoggingTarget.Network);

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
                    EventType.Register => handleListener<APIRegisterResponse>,
                    EventType.MultiplayerCreateLobby => handleListener<int>,
                    EventType.MultiplayerJoinLobby => handleListener<APIMultiplayerLobby>,
                    EventType.MultiplayerLobbyUpdate => handleListener<APIMultiplayerLobby>,
                    _ => () => { }
                };
                // execute handler
                handler();
            }
        }
        catch (Exception e)
        {
            Notifications.Post("Something went wrong while receiving data from server!");
            Logger.Error(e, "Something went wrong!", LoggingTarget.Network);
        }

        Notifications.Post("Disconnected from server!");
    }

    public static void Send(string message)
    {
        if (connection is not { State: WebSocketState.Open })
        {
            packet_queue.Add(message);
            return;
        }

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
        response_listeners.GetOrAdd(id, _ => new List<Action<object>>()).Add(response => listener((FluxelResponse<T>)response));
    }

    public static void UnregisterListener(EventType id)
    {
        response_listeners.Remove(id, out var listeners);
        listeners?.Clear();
    }

    public static void SetLoggedInUser(APIUser user)
    {
        OnUserLoggedIn?.Invoke(user);
        loggedInUser = user;
        Logger.Log($"Logged in as {user.Username}", LoggingTarget.Network);
    }

    [CanBeNull]
    public static APIUser GetLoggedInUser()
    {
        return loggedInUser;
    }

    public static void Reset()
    {
        loggedInUser = null;
        Token = null;
        response_listeners.Clear();
        packet_queue.Clear();
    }
}

public enum EventType
{
    Token = 0,
    Login = 1,
    Register = 2,

    MultiplayerCreateLobby = 20,
    MultiplayerJoinLobby = 21,
    MultiplayerLobbyUpdate = 22,
}
