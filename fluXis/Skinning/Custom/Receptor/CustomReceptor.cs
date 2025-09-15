using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Custom.Receptor;

public partial class CustomReceptor : ColorableSkinDrawable
{
    private int mode { get; }
    public bool SkipResizing { get; set; }

    protected Sprite Sprite { get; }

    public CustomReceptor(SkinJson skinJson, Texture texture, MapColor index, int mode)
        : base(skinJson, index)
    {
        this.mode = mode;

        RelativeSizeAxes = Axes.Both;

        InternalChild = Sprite = new Sprite
        {
            Texture = texture,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            RelativeSizeAxes = Axes.X,
            Width = 1,
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        switch (Index)
        {
            case MapColor.Primary:
                SetColor(ColorProvider.Primary);
                break;

            case MapColor.Secondary:
                SetColor(ColorProvider.Secondary);
                break;

            case MapColor.Middle:
                SetColor(ColorProvider.Middle);
                break;
        }
    }

    public override void SetColor(Colour4 color)
    {
        var keymode = SkinJson.GetKeymode(mode);

        if (!keymode.TintNotes)
            return;

        Sprite.Colour = color;
    }

    protected override void Update()
    {
        base.Update();

        Sprite.Height = Sprite.DrawWidth;

        if (SkipResizing)
            return;

        var receptorTexture = Sprite.Texture;

        if (receptorTexture != null)
        {
            switch (Sprite.RelativeSizeAxes)
            {
                case Axes.X:
                    Sprite.Height = DrawWidth / receptorTexture.DisplayWidth * receptorTexture.DisplayHeight;
                    break;

                case Axes.Y:
                    Sprite.Width = DrawHeight / receptorTexture.DisplayHeight * receptorTexture.DisplayWidth;
                    break;
            }
        }
    }
}
