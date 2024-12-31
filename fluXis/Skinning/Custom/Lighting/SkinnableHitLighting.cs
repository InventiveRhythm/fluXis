using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.Custom.Lighting;

public partial class SkinnableHitLighting : VisibilityContainer
{
    public SkinnableHitLighting(Texture texture)
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        RelativeSizeAxes = Axes.X;
        Alpha = 0;

        InternalChild = new Sprite
        {
            Texture = texture,
            RelativeSizeAxes = Axes.X,
            Width = 1,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            FillMode = FillMode.Fill
        };
    }

    public void SetColor(SkinJson json, int lane, int keyCount)
    {
        Colour = json.GetLaneColor(lane, keyCount);
    }

    protected override void PopIn() => this.FadeIn();
    protected override void PopOut() => this.FadeOut(300, Easing.Out);
}
