using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace fluXis.Game.Skinning;

public class CustomSkin : ISkin
{
    public Skin Skin { get; }
    public LargeTextureStore Textures { get; }
    public Storage Storage { get; }

    public CustomSkin(Skin skin, LargeTextureStore textures, Storage storage)
    {
        Skin = skin;
        Textures = textures;
        Storage = storage;
    }

    public Texture GetDefaultBackground()
    {
        const string path = "UserInterface/background.png";
        return Storage.Exists(path) ? Textures.Get(path) : null;
    }

    public Drawable GetStageBackground()
    {
        const string path = "Stage/background.png";

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new Sprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new SkinnableSprite
            {
                Texture = Textures.Get(path),
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

        if (Storage.Exists(path))
        {
            return new Sprite
            {
                Texture = Textures.Get(path),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            };
        }

        return null;
    }

    public Drawable GetJudgement(Judgement judgement)
    {
        var path = $"Judgement/{judgement.ToString().ToLower()}.png";
        return Storage.Exists(path) ? new Sprite { Texture = Textures.Get(path) } : null;
    }
}
