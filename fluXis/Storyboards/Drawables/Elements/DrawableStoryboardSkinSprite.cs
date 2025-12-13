using System;
using fluXis.Skinning;
using fluXis.Skinning.Default.Receptor;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardSkinSprite : DrawableStoryboardElement
{
    public DrawableStoryboardSkinSprite(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.SkinSprite)
            throw new ArgumentException($"Element provided is not {nameof(StoryboardElementType.SkinSprite)}", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load(ISkin skin)
    {
        var spr = Element.GetParameter("sprite", SkinSprite.HitObject);
        var lane = Element.GetParameter("lane", 0);
        var keycount = Element.GetParameter("keycount", 0);

        InternalChild = spr switch
        {
            SkinSprite.HitObject => getSprite(skin.GetHitObject(lane, keycount)).With(x => x.RelativeSizeAxes = Axes.X),
            SkinSprite.LongNoteStart => getSprite(skin.GetLongNoteStart(lane, keycount)).With(x => x.RelativeSizeAxes = Axes.X),
            SkinSprite.LongNoteBody => getSprite(skin.GetLongNoteBody(lane, keycount)).With(x => x.RelativeSizeAxes = Axes.Both),
            SkinSprite.LongNoteEnd => getSprite(skin.GetLongNoteStart(lane, keycount)).With(x => x.RelativeSizeAxes = Axes.X),
            SkinSprite.TickNote => getSprite(skin.GetTickNote(lane, keycount, false)).With(x => x.RelativeSizeAxes = Axes.X),
            SkinSprite.TickNoteSmall => getSprite(skin.GetTickNote(lane, keycount, true)).With(t =>
            {
                t.RelativeSizeAxes = Axes.X;

                // this needs to be centered because default bar changes the width directly
                var y = Origin & (Anchor.y0 | Anchor.y1 | Anchor.y2);
                t.Anchor = t.Origin = y | Anchor.x1;
            }),
            SkinSprite.Receptor => getSprite(skin.GetReceptor(lane, keycount, false))
                .With(r => r.RelativeSizeAxes = r is DefaultReceptorUp ? Axes.Both : Axes.X),

            SkinSprite.StageBackground => getSprite(skin.GetStageBackgroundPart(Anchor.Centre)),
            SkinSprite.StageBackgroundTop => getSprite(skin.GetStageBackgroundPart(Anchor.TopCentre)),
            SkinSprite.StageBackgroundBottom => getSprite(skin.GetStageBackgroundPart(Anchor.BottomCentre)),
            SkinSprite.StageLeft => getSprite(skin.GetStageBackgroundPart(Anchor.CentreLeft)),
            SkinSprite.StageLeftTop => getSprite(skin.GetStageBackgroundPart(Anchor.TopLeft)),
            SkinSprite.StageLeftBottom => getSprite(skin.GetStageBackgroundPart(Anchor.BottomLeft)),
            SkinSprite.StageRight => getSprite(skin.GetStageBackgroundPart(Anchor.CentreRight)),
            SkinSprite.StageRightTop => getSprite(skin.GetStageBackgroundPart(Anchor.TopRight)),
            SkinSprite.StageRightBottom => getSprite(skin.GetStageBackgroundPart(Anchor.BottomRight)),

            _ => throw new ArgumentException()
        };

        return;

        Drawable getSprite(Drawable sprite) => sprite.With(x =>
        {
            x.Anchor = Origin;
            x.Origin = Origin;
        });
    }
}
