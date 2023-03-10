using System;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class JudgementDisplay : GameplayHUDElement
{
    public JudgementDisplay(GameplayScreen screen)
        : base(screen)
    {
        screen.Performance.OnHitStatAdded += PopUp;
    }

    private SpriteText text;
    private SpriteText textEarlyLate;
    private Bindable<bool> hideFlawless;
    private Bindable<bool> showEarlyLate;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        hideFlawless = config.GetBindable<bool>(FluXisSetting.HideFlawless);
        showEarlyLate = config.GetBindable<bool>(FluXisSetting.ShowEarlyLate);

        Anchor = Anchor.TopCentre;
        Origin = Anchor.Centre;
        RelativePositionAxes = Axes.Y;
        Y = 0.6f;

        InternalChildren = new Drawable[]
        {
            text = new SpriteText
            {
                Font = new FontUsage("Quicksand", 48f, "Bold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive
            },
            textEarlyLate = new SpriteText
            {
                Font = new FontUsage("Quicksand", 24f, "Bold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive,
                Margin = new MarginPadding { Top = 48 },
            }
        };
    }

    public void PopUp(HitStat stat)
    {
        var hitWindow = HitWindow.FromKey(stat.Judgement);
        var judgement = stat.Judgement;

        if (hideFlawless.Value && judgement == Scoring.Judgement.Flawless) return;

        const int random_angle = 7;
        float scale = 1.4f;
        float rotation = 0;

        if (judgement == Scoring.Judgement.Miss)
        {
            scale = 1.8f;
            rotation = new Random().Next(-random_angle, random_angle);
        }

        text.Text = judgement.ToString();
        text.Colour = hitWindow.Color;
        text.RotateTo(rotation)
            .ScaleTo(1f)
            .TransformSpacingTo(new Vector2(0, 0))
            .ScaleTo(scale, 1000, Easing.OutQuint)
            .FadeOutFromOne(500)
            .TransformSpacingTo(new Vector2(5, 0), 1000, Easing.OutQuint);

        if (showEarlyLate.Value && judgement != Scoring.Judgement.Flawless && judgement != Scoring.Judgement.Miss)
        {
            bool early = stat.Difference < 0;
            textEarlyLate.Text = early ? "Early" : "Late";
            textEarlyLate.Colour = early ? Colour4.FromHex("#37cbfb") : Colour4.FromHex("#fb9d37");

            textEarlyLate.ScaleTo(1f)
                         .TransformSpacingTo(new Vector2(0, 0))
                         .ScaleTo(scale, 1000, Easing.OutQuint)
                         .FadeOutFromOne(500)
                         .TransformSpacingTo(new Vector2(5, 0), 1000, Easing.OutQuint);
        }
        else textEarlyLate.FadeOut();
    }
}
