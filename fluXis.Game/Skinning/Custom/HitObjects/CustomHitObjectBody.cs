using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Bases.HitObjects;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Skinning.Custom.HitObjects;

public partial class CustomHitObjectBody : ColorableSkinDrawable, ICanHaveSnapColor
{
    private int mode { get; }
    private Drawable sprite { get; }

    public CustomHitObjectBody(SkinJson skinJson, int mode, Texture texture)
        : base(skinJson)
    {
        this.mode = mode;

        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChild = sprite = new Sprite
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(1),
            Texture = texture
        };
    }

    protected override void SetColor(Colour4 color)
    {
        var keymode = SkinJson.GetKeymode(mode);

        if (!keymode.TintNotes || !keymode.TintLongNotes)
            return;

        sprite.Colour = color;
    }

    public void ApplySnapColor(int start, int end)
    {
        var keymode = SkinJson.GetKeymode(mode);

        if (!keymode.TintNotes || !keymode.TintLongNotes)
            return;

        UseCustomColor = true;
        var startColor = SkinJson.SnapColors.GetColor(start);
        var endColor = SkinJson.SnapColors.GetColor(end);
        sprite.Colour = ColourInfo.GradientVertical(endColor, startColor);
    }
}
