using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Graphics.Textures;
using osu.Framework.Text;

namespace fluXis.Online.Chat;

public class EmojiIconsStore : ITextureStore, ITexturedGlyphLookupStore
{
    public const string ICON_FONT_NAME = "emoji-icons";

    private readonly ChatDecoManager deco;
    private readonly TextureStore textures;

    public EmojiIconsStore(ChatDecoManager deco, TextureStore textures)
    {
        this.deco = deco;
        this.textures = textures;
    }

    public Texture Get(string name) => null;
    public Task<Texture> GetAsync(string name, CancellationToken cancellationToken = default) => null;
    public Stream GetStream(string name) => null;
    public IEnumerable<string> GetAvailableResources() => null;
    public Texture Get(string name, WrapMode wrapModeS, WrapMode wrapModeT) => null;
    public Task<Texture> GetAsync(string name, WrapMode wrapModeS, WrapMode wrapModeT, CancellationToken cancellationToken = default) => null;
    public ITexturedCharacterGlyph Get(string fontName, char character)
        => fontName == ICON_FONT_NAME ? new Icon(textures.Get($"Emoji/{deco.EmojiIndexes[character]}")) : null;

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
