using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning.Custom.HitObjects;

public partial class CustomHitObjectPiece : ColorableSkinDrawable
{
    private int mode { get; }
    private Drawable sprite { get; }

    public CustomHitObjectPiece(SkinJson skinJson, int mode, Texture texture)
        : base(skinJson)
    {
        this.mode = mode;

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChild = sprite = new SkinnableSprite
        {
            RelativeSizeAxes = Axes.X,
            Texture = texture,
            Width = 1
        };
    }

    protected override void SetColor(Colour4 color)
    {
        if (!SkinJson.GetKeymode(mode).TintNotes)
            return;

        sprite.Colour = color;
    }
}
