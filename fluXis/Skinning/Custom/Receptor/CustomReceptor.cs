using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Custom.Receptor;

public partial class CustomReceptor : ColorableSkinDrawable
{
    private Drawable sprite { get; }
    private int mode { get; }

    public CustomReceptor(SkinJson skinJson, MapColor index, int mode, Texture texture, [CanBeNull] Texture tintless)
        : base(skinJson, index)
    {
        this.mode = mode;

        RelativeSizeAxes = Axes.Both;

        InternalChild = sprite = new SkinnableSprite
        {
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Texture = texture,
            Width = 1
        };

        if (tintless != null)
        {
            AddInternal(new SkinnableSprite
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Texture = tintless,
                Width = 1
            });
        }
    }

    public override void SetColor(Colour4 color)
    {
        var keymode = SkinJson.GetKeymode(mode);

        if (!keymode.TintReceptors)
            return;

        sprite.Colour = color;
    }
}
