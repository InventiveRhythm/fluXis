using fluXis.Game.Overlay.Notifications.Floating;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Other;

public class ServerMessage
{
    /// <summary>
    /// The text to display.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; init; }

    /// <summary>
    /// The subtext to display.
    /// Only used when <see cref="ServerMessage.NonDisruptive"/> is <see langword="false"/>.
    /// </summary>
    [JsonProperty("sub_text")]
    public string SubText { get; init; }

    /// <summary>
    /// Whether this message should be displayed as a <see cref="SmallFloatingTextNotification"/>.
    /// </summary>
    [JsonProperty("non_disruptive")]
    public bool NonDisruptive { get; init; }
}
