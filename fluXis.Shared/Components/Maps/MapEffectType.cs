namespace fluXis.Shared.Components.Maps;

[Flags]
public enum MapEffectType : ulong
{
    ScrollVelocity = 1 << 0, // 1
    LaneSwitch = 1 << 1, // 2
    Flash = 1 << 2, // 4
    Pulse = 1 << 3, // 8
    PlayfieldMove = 1 << 4, // 16
    PlayfieldScale = 1 << 5, // 32
    PlayfieldRotate = 1 << 6, // 64
    PlayfieldFade = 1 << 7, // 128
    Shake = 1 << 8, // 256
    Shader = 1 << 9, // 512
    BeatPulse = 1 << 10, // 1024
}

