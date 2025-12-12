using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Default;
using fluXis.Skinning.Default.Stage;
using fluXis.Skinning.DefaultCircle.HitObject;
using fluXis.Skinning.DefaultCircle.Lighting;
using fluXis.Skinning.DefaultCircle.Receptor;
using fluXis.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning.DefaultCircle;

public class DefaultCircleSkin : DefaultSkin
{
    public const float SCALE = .9f;

    public DefaultCircleSkin(TextureStore textures, ISampleStore samples)
        : base(textures, samples)
    {
    }

    public override Texture GetIcon() => Textures.Get("Skins/circle.png");

    public override Drawable GetHitObject(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var piece = new DefaultCircleHitObjectPiece(SkinJson, (MapColor)index);
        piece.UpdateColor(lane, keyCount);
        return piece;
    }

    public override Drawable GetLongNoteStart(int lane, int keyCount) => GetHitObject(lane, keyCount);

    public override Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var body = new DefaultCircleHitObjectBody(SkinJson, (MapColor)index);
        body.BoxSprite.Width = 1f;
        body.UpdateColor(lane, keyCount);
        return body;
    }

    public override Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var end = new DefaultCircleHitObjectEnd(SkinJson, (MapColor)index);
        end.UpdateColor(lane, keyCount);
        return end;
    }

    public override Drawable GetStageBackgroundPart(Anchor part)
    {
        if (part == Anchor.BottomCentre)
        {
            return new DefaultStageBackgroundBottom
            {
                Colour = ColourInfo.GradientVertical(Colour4.Black, Colour4.Black.Opacity(0)),
                Alpha = 0.8f
            };
        }

        return base.GetStageBackgroundPart(part);
    }

    public override Drawable GetTickNote(int lane, int keyCount, bool small) => new DefaultCircleTickNote(small);
    public override VisibilityContainer GetColumnLighting(int lane, int keyCount) => new DefaultCircleColumnLighting();

    public override Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        var receptor = down ? new DefaultCircleReceptorDown(SkinJson, (MapColor)index) : new DefaultCircleReceptorUp(SkinJson, (MapColor)index);
        receptor.UpdateColor(lane, keyCount);
        return receptor;
    }

    public override Drawable GetHitLine() => Drawable.Empty();

    protected override SkinJson CreateJson()
    {
        // too lazy to find a better way
        var json = base.CreateJson();
        json.Info = new SkinInfo
        {
            Name = SkinManager.DEFAULT_CIRCLE_SKIN_NAME,
            Creator = "flustix",
            Path = SkinManager.DEFAULT_CIRCLE_SKIN_NAME
        };
        json.OneKey = json.TwoKey = json.ThreeKey = json.FourKey = json.FiveKey =
            json.SixKey = json.SevenKey = json.EightKey = json.NineKey = json.TenKey = create();
        return json;

        SkinKeymode create() => new()
        {
            ColumnWidth = 128,
            HitPosition = 0,
            ReceptorsFirst = true
        };
    }
}
