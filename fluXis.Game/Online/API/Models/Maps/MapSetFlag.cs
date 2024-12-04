using System;

namespace fluXis.Game.Online.API.Models.Maps;

[Flags]
public enum MapSetFlag : ulong
{
    /// <summary>
    /// song/background contains swearing or similar
    /// </summary>
    Explicit = 1 << 0,

    /// <summary>
    /// song is in the featured artist library
    /// </summary>
    FeaturedArtist = 1 << 1,
}
