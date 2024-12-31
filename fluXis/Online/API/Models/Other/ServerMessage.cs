using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Other;

public class ServerMessage
{
    /// <summary>
    /// The type of message.
    /// <br/>
    /// normal: A <c>FloatingTextNotification</c>;
    /// <br/>
    /// small: A <c>SmallFloatingTextNotification</c>;
    /// <br/>
    /// image: A <c>OnlineImageNotification</c>
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; init; } = "normal";

    /// <summary>
    /// The text to display.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; init; } = "";

    /// <summary>
    /// Extra text for the message.
    /// Only used when <see cref="ServerMessage.Type"/> is <c>normal</c>.
    /// </summary>
    [JsonProperty("sub_text")]
    public string SubText { get; init; } = "";

    /// <summary>
    /// The path to the image to display.
    /// Only used when <see cref="ServerMessage.Type"/> is <c>image</c>.
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; init; } = "";
}
