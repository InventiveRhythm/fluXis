using fluXis.Game.Overlay.Notifications.Floating;
using fluXis.Game.Overlay.Notifications.Types.Image;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Other;

public class ServerMessage
{
    /// <summary>
    /// The type of message.
    /// <br/>
    /// normal: A <see cref="FloatingTextNotification"/>
    /// <br/>
    /// small: A <see cref="SmallFloatingTextNotification"/>
    /// <br/>
    /// image: A <see cref="OnlineImageNotification"/>
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; init; } = "normal";

    /// <summary>
    /// The text to display.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; init; }

    /// <summary>
    /// Extra text for the message.
    /// Only used when <see cref="ServerMessage.Type"/> is <c>normal</c>.
    /// </summary>
    [JsonProperty("sub_text")]
    public string SubText { get; init; }

    /// <summary>
    /// The path to the image to display.
    /// Only used when <see cref="ServerMessage.Type"/> is <c>image</c>.
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; init; }
}
