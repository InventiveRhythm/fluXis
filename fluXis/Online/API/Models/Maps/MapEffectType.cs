using System;

namespace fluXis.Online.API.Models.Maps;

[Flags]
public enum MapEffectType : ulong
{
    ScrollVelocity = 1 << 0,
    LaneSwitch = 1 << 1,
    Flash = 1 << 2,
    Pulse = 1 << 3,
    PlayfieldMove = 1 << 4,
    PlayfieldScale = 1 << 5,
    PlayfieldRotate = 1 << 6,

    // UNUSED //
    PlayfieldFade = 1 << 7,

    Shake = 1 << 8,
    Shader = 1 << 9,
    BeatPulse = 1 << 10,
    LayerFade = 1 << 11,
    HitObjectEase = 1 << 12,
    ScrollMultiply = 1 << 13,
    TimeOffset = 1 << 14,

    ColorFade = 1 << 15
}

