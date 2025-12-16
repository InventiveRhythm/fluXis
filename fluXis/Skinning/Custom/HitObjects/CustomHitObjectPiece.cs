using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Bases.HitObjects;
using fluXis.Skinning.Json;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Custom.HitObjects;

public partial class CustomHitObjectPiece : ColorableSkinDrawable, ICanHaveSnapColor
{
    private int mode { get; }
    private bool isEnd { get; }
    private Drawable sprite { get; }

    public CustomHitObjectPiece(SkinJson skinJson, MapColor index, int mode, bool end, Texture texture, [CanBeNull] Texture tintless)
        : base(skinJson, index)
    {
        this.mode = mode;
        isEnd = end;

        AutoSizeAxes = Axes.Y;

        InternalChild = sprite = new SkinnableSprite
        {
            RelativeSizeAxes = Axes.X,
            Texture = texture,
            Width = 1
        };

        if (tintless != null)
        {
            AddInternal(new SkinnableSprite
            {
                RelativeSizeAxes = Axes.X,
                Texture = tintless,
                Width = 1
            });
        }
    }

    public override void SetColor(Colour4 color)
    {
        var keymode = SkinJson.GetKeymode(mode);

        if (!keymode.TintNotes)
            return;

        if (isEnd && !keymode.TintLongNotes)
            return;

        sprite.Colour = color;
    }

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        SetColor(SkinJson.SnapColors.GetColor(start));
    }
}
