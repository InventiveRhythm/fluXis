using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Scoring.Enums;
using fluXis.Skinning.Bases.Judgements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Skinning.Default.Judgements;

public partial class DefaultJudgementText : AbstractJudgementText
{
    private FluXisSpriteText judgementText;
    private FluXisSpriteText earlyLateText;

    public DefaultJudgementText(Judgement judgement, bool isLate)
        : base(judgement, isLate)
    {
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        AutoSizeAxes = Axes.X;
        Height = 48;

        InternalChildren = new Drawable[]
        {
            judgementText = new FluXisSpriteText
            {
                FontSize = 48,
                Alpha = 0,
                Text = Judgement.ToString(),
                Colour = skinManager.SkinJson.GetColorForJudgement(Judgement),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            earlyLateText = new FluXisSpriteText
            {
                FontSize = 24,
                Alpha = 0,
                Text = IsLate ? "LATE" : "EARLY",
                Colour = IsLate ? Colour4.FromHex("#fb9d37") : Colour4.FromHex("#37cbfb"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding { Top = 48 }
            }
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
        var spacing = new Vector2(5, 0);

        judgementText.FadeIn().ScaleTo(Vector2.One).TransformSpacingTo(Vector2.Zero).RotateTo(0)
                     .ScaleTo(scale, total_duration, Easing.OutQuint).RotateTo(rotation)
                     .TransformSpacingTo(spacing, total_duration, Easing.OutQuint)
                     .Delay(delay).FadeOut(fade_duration);

        if (!showEarlyLate || !ShouldShowEarlyLate)
            return;

        earlyLateText.FadeIn().ScaleTo(Vector2.One).TransformSpacingTo(Vector2.Zero)
                     .ScaleTo(scale, 1000, Easing.OutQuint)
                     .TransformSpacingTo(spacing, total_duration, Easing.OutQuint)
                     .Delay(delay).FadeOut(fade_duration);
    }
}
