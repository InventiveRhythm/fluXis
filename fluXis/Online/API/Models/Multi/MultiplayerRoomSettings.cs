namespace fluXis.Online.API.Models.Multi;

public class MultiplayerRoomSettings : IMultiplayerRoomSettings
{
    public string Name { get; set; } = string.Empty;
    public bool HasPassword { get; set; }
}
