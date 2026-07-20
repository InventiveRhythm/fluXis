using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Text;

namespace fluXis.Graphics.Sprites.Icons;

public static partial class FluXisIcon
{
    public const string ICON_FONT_NAME = "fluXis-Icons";
    public static IconUsage Get(FluXisIconType type) => new((char)type, ICON_FONT_NAME);

    public class Store : ITextureStore, ITexturedGlyphLookupStore
    {
        private readonly TextureStore textures;

        public Store(TextureStore textures)
        {
            this.textures = textures;
        }

        public Texture Get(string name) => null;
        public Task<Texture> GetAsync(string name, CancellationToken cancellationToken = default) => null;
        public Stream GetStream(string name) => null;
        public IEnumerable<string> GetAvailableResources() => null;
        public Texture Get(string name, WrapMode wrapModeS, WrapMode wrapModeT) => null;
        public Task<Texture> GetAsync(string name, WrapMode wrapModeS, WrapMode wrapModeT, CancellationToken cancellationToken = default) => null;

        public ITexturedCharacterGlyph Get(string fontName, char character)
            => fontName == ICON_FONT_NAME ? new Icon(textures.Get($"Icons/{((FluXisIconType)character).GetDescription()}")) : null;

        public Task<ITexturedCharacterGlyph> GetAsync(string fontName, char character) => Task.Run(() => Get(fontName, character));

        public void Dispose() => textures.Dispose();

        public class Icon : ITexturedCharacterGlyph
        {
            public float XOffset => 0;
            public float YOffset => 0;
            public float XAdvance => 0;
            public float Baseline => 0;
            public char Character => '\0';

            public Texture Texture { get; }
            public float Width => Texture.Width;
            public float Height => Texture.Height;

            public Icon(Texture texture)
            {
                Texture = texture;
            }

            public float GetKerning<T>(T lastGlyph) where T : ICharacterGlyph => 0f;
        }
    }
}

public enum FluXisIconType
{
    BeatPulse,
    Drum,
    Flash,
    HitObjectEase,
    LaneSwitch,
    LayerFade,
    PlayfieldFade,
    PlayfieldMove,
    PlayfieldRotate,
    PlayfieldScale,
    Pulse,
    ScrollMultiply,
    Shader,
    Shake,
    TimeOffset,
}
