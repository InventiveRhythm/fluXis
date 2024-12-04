using System;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning.Bases.Judgements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Skinning.Custom.Judgements;

public partial class SkinnableJudgementText : AbstractJudgementText
{
    private Texture texture { get; }

    private Sprite sprite;

    public SkinnableJudgementText(Texture texture, Judgement judgement, bool isLate)
        : base(judgement, isLate)
    {
        this.texture = texture;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = sprite = new Sprite
        {
            Alpha = 0,
            Texture = texture,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }

    public override void Animate(bool showEarlyLate)
    {
        const int random_angle = 7;
        const float total_duration = 1000;
        const float delay = 600;
        const float fade_duration = 400;

        var rotation = Judgement == Judgement.Miss ? new Random().Next(-random_angle, random_angle) : 0;
        var scale = Judgement == Judgement.Miss ? 1.8f : 1.4f;

        sprite.FadeIn().ScaleTo(Vector2.One).RotateTo(0)
              .ScaleTo(scale, total_duration, Easing.OutQuint).RotateTo(rotation)
              .Delay(delay).FadeOut(fade_duration);
    }
}
