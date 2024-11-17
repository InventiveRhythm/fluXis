using MessagePack;

namespace fluXis.Shared.Spectator;

[MessagePackObject]
public class SpectatorState
{
    [Key(0)]
    public long? MapID { get; set; }
}
