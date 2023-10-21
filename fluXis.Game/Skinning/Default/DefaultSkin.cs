using System;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Default.HitObject;
using fluXis.Game.Skinning.Default.Lighting;
using fluXis.Game.Skinning.Default.Receptor;
using fluXis.Game.Skinning.Default.Stage;
using fluXis.Game.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning.Default;

public class DefaultSkin : ISkin
{
    public SkinJson SkinJson { get; init; }
    private TextureStore textures { get; }
    private ISampleStore samples { get; }

    public DefaultSkin(TextureStore textures, ISampleStore samples)
    {
        SkinJson = new SkinJson();
        this.textures = textures;
        this.samples = samples;
    }

    public Texture GetDefaultBackground() => textures.Get("Backgrounds/default.png");
    public Drawable GetStageBackground() => new DefaultStageBackground();
    public Drawable GetStageBorder(bool right) => right ? new DefaultStageBorderRight() : new DefaultStageBorderLeft();
    public Drawable GetLaneCover(bool bottom) => bottom ? new DefaultBottomLaneCover() : new DefaultTopLaneCover();

    public Drawable GetHitObject(int lane, int keyCount)
    {
        var piece = new DefaultHitObjectPiece();
        piece.UpdateColor(lane, keyCount);
        return piece;
    }

    public Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var body = new DefaultHitObjectBody();
        body.UpdateColor(lane, keyCount);
        return body;
    }

    public Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var end = new DefaultHitObjectEnd();
        end.UpdateColor(lane, keyCount);
        return end;
    }

    public Drawable GetColumnLighting(int lane, int keyCount)
    {
        var lighting = new DefaultColumnLighing();
        lighting.UpdateColor(lane, keyCount);
        return lighting;
    }

    public Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var receptor = down ? new DefaultReceptorDown() : new DefaultReceptorUp();
        receptor.UpdateColor(lane, keyCount);
        receptor.Height = SkinJson.GetKeymode(keyCount).HitPosition;
        return receptor;
    }

    public Drawable GetHitLine() => new DefaultHitLine();
    public Drawable GetJudgement(Judgement judgement) => null;

    public Sample GetHitSample() => samples.Get("Gameplay/hitsound");
    public Sample[] GetMissSamples() => new[] { samples.Get("Gameplay/combobreak") };
    public Sample GetFailSample() => samples.Get("Gameplay/fail");
    public Sample GetRestartSample() => samples.Get("Gameplay/restart");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        textures?.Dispose();
        samples?.Dispose();
    }
}
