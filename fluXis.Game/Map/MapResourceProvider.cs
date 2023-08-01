using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Map;

public class MapResourceProvider
{
    public TextureStore BackgroundStore { get; init; }
    public TextureStore CroppedBackgroundStore { get; init; }
    public ITrackStore TrackStore { get; init; }
}
