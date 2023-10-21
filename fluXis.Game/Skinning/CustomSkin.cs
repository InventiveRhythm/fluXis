using System;
using System.Linq;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace fluXis.Game.Skinning;

public class CustomSkin : ISkin
{
    public SkinJson SkinJson { get; }
    private LargeTextureStore textures { get; }
    private Storage storage { get; }
    private ISampleStore samples { get; }

    public CustomSkin(SkinJson skinJson, LargeTextureStore textures, Storage storage, ISampleStore samples)
    {
        SkinJson = skinJson;
        this.textures = textures;
        this.storage = storage;
        this.samples = samples;
    }

    public Texture GetDefaultBackground()
    {
        const string path = "UserInterface/background.png";
        return storage.Exists(path) ? textures.Get(path) : null;
    }

    public Drawable GetStageBackground()
    {
        const string path = "Stage/background.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetStageBorder(bool right)
    {
        var path = $"Stage/border-{(right ? "right" : "left")}.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = right ? Anchor.TopRight : Anchor.TopLeft,
                Origin = right ? Anchor.TopLeft : Anchor.TopRight,
                RelativeSizeAxes = Axes.Y,
                Height = 1
            };
        }

        return null;
    }

    public Drawable GetLaneCover(bool bottom)
    {
        var path = $"Stage/lane-cover-{(bottom ? "bottom" : "top")}.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                Origin = bottom ? Anchor.BottomCentre : Anchor.TopCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetHitObject(int lane, int keyCount)
    {
        var path = $"HitObjects/Note/{keyCount}k-{lane}.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetLongNoteBody(int lane, int keyCount)
    {
        var path = $"HitObjects/LongNoteBody/{keyCount}k-{lane}.png";

        if (storage.Exists(path))
        {
            return new Sprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetLongNoteEnd(int lane, int keyCount)
    {
        var path = $"HitObjects/LongNoteEnd/{keyCount}k-{lane}.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetColumnLighting(int lane, int keyCount)
    {
        const string path = "Lighting/column-lighting.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetReceptor(int lane, int keyCount, bool down)
    {
        var path = $"Receptor/{keyCount}k-{lane}-{(down ? "down" : "up")}.png";

        if (storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1
            };
        }

        return null;
    }

    public Drawable GetHitLine()
    {
        const string path = "Stage/hitline.png";

        if (storage.Exists(path))
        {
            return new Sprite
            {
                Texture = textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            };
        }

        return null;
    }

    public Drawable GetJudgement(Judgement judgement)
    {
        var path = $"Judgement/{judgement.ToString().ToLower()}.png";
        return storage.Exists(path) ? new Sprite { Texture = textures.Get(path) } : null;
    }

    public Sample GetHitSample() => samples.Get("Samples/Gameplay/hit");

    public Sample[] GetMissSamples()
    {
        if (!storage.ExistsDirectory("Samples/Gameplay")) return null;

        var missSamples = storage.GetFiles("Samples/Gameplay", "miss*")
                                 .Select(file => samples.Get($"{file}"))
                                 .Where(sample => sample != null).ToList();

        return missSamples.Count == 0 ? null : missSamples.ToArray();
    }

    public Sample GetFailSample() => samples.Get("Samples/Gameplay/fail");
    public Sample GetRestartSample() => samples.Get("Samples/Gameplay/restart");

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        textures?.Dispose();
        samples?.Dispose();
    }
}
