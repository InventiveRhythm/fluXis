using fluXis.Skinning.Default;
using fluXis.Skinning.DefaultCircle.HitObject;
using fluXis.Skinning.DefaultCircle.Lighting;
using fluXis.Skinning.DefaultCircle.Receptor;
using fluXis.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
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

    public override Drawable GetHitObject(int lane, int keyCount)
    {
        var piece = new DefaultCircleHitObjectPiece(SkinJson);
        piece.UpdateColor(lane, keyCount);
        return piece;
    }

    public override Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var body = new DefaultCircleHitObjectBody(SkinJson);
        body.UpdateColor(lane, keyCount);
        return body;
    }

    public override Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var end = new DefaultCircleHitObjectEnd(SkinJson);
        end.UpdateColor(lane, keyCount);
        return end;
    }

    public override Drawable GetTickNote(int lane, int keyCount, bool small) => new DefaultCircleTickNote(small);
    public override VisibilityContainer GetColumnLighting(int lane, int keyCount) => new DefaultCircleColumnLighting();

    public override Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var receptor = down ? new DefaultCircleReceptorDown(SkinJson) : new DefaultCircleReceptorUp(SkinJson);
        receptor.UpdateColor(lane, keyCount);
        return receptor;
    }

    public override Drawable GetHitLine() => Drawable.Empty();

    protected override SkinJson CreateJson()
    {
        // too lazy to find a better way
        var json = base.CreateJson();
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
