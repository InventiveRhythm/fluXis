using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel.Packets;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel;

public class Fluxel
{
    internal static HttpClient Http = new();
    private static ClientWebSocket connection;

    private static int retryCount;
    private static bool reconnecting;
    private const int max_retries = 5;

    private static readonly List<string> packet_queue = new();

    public static string Token;
    public static Action<APIUserShort> OnUserLoggedIn;

    private static APIUserShort loggedInUser;

    public static APIUserShort LoggedInUser
    {
        get => loggedInUser;
        set
        {
            loggedInUser = value;
            OnUserLoggedIn?.Invoke(loggedInUser);
            Logger.Log($"Logged in as {value.Username}", LoggingTarget.Network);
        }
    }

    public static NotificationOverlay Notifications;

    private static readonly ConcurrentDictionary<EventType, List<Action<object>>> response_listeners = new();

    public static async void Connect()
    {
        try
        {
            if (reconnecting)
                Notifications.Post($"Reconnecting to server... (attempt {retryCount}/{max_retries})");

            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(APIConstants.WebsocketUrl), CancellationToken.None);

            // create thread
            receive();

            if (reconnecting)
                Notifications.Post("Reconnected to server!");

            // send queued packets
            foreach (var packet in packet_queue)
                Send(packet);

            reconnecting = false;
        }
        catch (Exception ex)
        {
            Notifications.PostError("Failed to connect to server!");
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);

            reconnect();
        }
    }

    private static void reconnect()
    {
        reconnecting = true;

        if (retryCount < max_retries)
        {
            retryCount++;
            Connect();
        }
        else
            Notifications.PostError("Failed to connect to server! Please try again later.");
    }

    private static async void receive()
    {
        while (connection.State == WebSocketState.Open)
        {
            try
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
                    EventType.Login => handleListener<APIUserShort>,
                    EventType.Register => handleListener<APIRegisterResponse>,
                    EventType.MultiplayerCreateLobby => handleListener<int>,
                    EventType.MultiplayerJoinLobby => handleListener<APIMultiplayerLobby>,
                    EventType.MultiplayerLobbyUpdate => handleListener<APIMultiplayerLobby>,
                    _ => () => { }
                };
                // execute handler
                handler();
            }
            catch (Exception e)
            {
                Notifications.PostError("Something went wrong while receiving data from server!");
                Logger.Error(e, "Something went wrong!", LoggingTarget.Network);
            }
        }

        Notifications.PostError("Disconnected from server!");
        reconnect();
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
