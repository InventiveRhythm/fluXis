using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Integration;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class JudgementDisplay : GameplayHUDElement
{
    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    public JudgementDisplay(GameplayScreen screen)
        : base(screen)
    {
        screen.Performance.OnHitStatAdded += PopUp;
    }

    private FluXisSpriteText text;
    private FluXisSpriteText textEarlyLate;
    private Container skinnableTextContainer;

    private Bindable<bool> hideFlawless;
    private Bindable<bool> showEarlyLate;
    private Bindable<bool> judgementSplash;

    private CircularContainer circle;
    private Splash splash;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        hideFlawless = config.GetBindable<bool>(FluXisSetting.HideFlawless);
        showEarlyLate = config.GetBindable<bool>(FluXisSetting.ShowEarlyLate);
        judgementSplash = config.GetBindable<bool>(FluXisSetting.JudgementSplash);

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Y = 150;

        InternalChildren = new Drawable[]
        {
            circle = new CircularContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(100),
                Masking = true,
                BorderThickness = 5,
                Alpha = 0,
                Child = new Box
                {
                    AlwaysPresent = true,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                }
            },
            splash = new Splash(),
            text = new FluXisSpriteText
            {
                FontSize = 48,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            skinnableTextContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            textEarlyLate = new FluXisSpriteText
            {
                FontSize = 24,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
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

        var skinTexture = skinManager.GetJudgement(judgement);

        if (skinTexture != null)
        {
            text.FadeOut();

            skinnableTextContainer.Clear();
            skinnableTextContainer.Add(skinTexture);

            skinnableTextContainer.RotateTo(rotation).ScaleTo(1f).FadeIn()
                                  .ScaleTo(scale, 1000, Easing.OutQuint)
                                  .Delay(600).FadeOut(400);
        }
        else
        {
            skinnableTextContainer.FadeOut();

            text.Text = judgement.ToString();
            text.Colour = skinManager.CurrentSkin.GetColorForJudgement(judgement);
            text.RotateTo(rotation).ScaleTo(1f).FadeIn().TransformSpacingTo(new Vector2(0, 0))
                .ScaleTo(scale, 1000, Easing.OutQuint)
                .TransformSpacingTo(new Vector2(5, 0), 1000, Easing.OutQuint)
                .Delay(600).FadeOut(400);
        }

        if (judgementSplash.Value)
        {
            circle.BorderColour = skinManager.CurrentSkin.GetColorForJudgement(judgement);
            circle.FadeInFromZero(200)
                  .TransformTo(nameof(circle.BorderThickness), 20f)
                  .TransformTo(nameof(circle.BorderThickness), 0f, 700, Easing.OutQuint)
                  .ScaleTo(0f)
                  .ScaleTo(1.4f, 500, Easing.OutQuint);

            if (judgement != Scoring.Judgement.Miss)
            {
                splash.Colour = skinManager.CurrentSkin.GetColorForJudgement(judgement);
                splash.Splat();
            }
        }

        lightController.FadeColour(skinManager.CurrentSkin.GetColorForJudgement(judgement))
                       .FadeColour(Color4.Black, 400);

        if (showEarlyLate.Value && judgement != Scoring.Judgement.Flawless && judgement != Scoring.Judgement.Miss)
        {
            bool early = stat.Difference > 0;
            textEarlyLate.Text = early ? "Early" : "Late";
            textEarlyLate.Colour = early ? Colour4.FromHex("#37cbfb") : Colour4.FromHex("#fb9d37");

            textEarlyLate.ScaleTo(1f)
                         .FadeIn()
                         .TransformSpacingTo(new Vector2(0, 0))
                         .ScaleTo(scale, 1000, Easing.OutQuint)
                         .TransformSpacingTo(new Vector2(5, 0), 1000, Easing.OutQuint)
                         .Delay(600).FadeOut(400);
        }
        else textEarlyLate.FadeOut();
    }

    private partial class Splash : Container<CircularContainer>
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            for (int i = 0; i < 10; i++)
            {
                AddInternal(new CircularContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(20),
                    Masking = true,
                    Alpha = 0,
                    Child = new Box
                    {
                        AlwaysPresent = true,
                        RelativeSizeAxes = Axes.Both
                    }
                });
            }
        }

        public void Splat()
        {
            foreach (var circle in InternalChildren)
            {
                var randomVelocity = new Vector2(RNG.NextSingle(-1f, 1f), RNG.NextSingle(-1f, 1f));
                var randomScale = RNG.NextSingle(0.5f, 1f);
                var randomSpeed = RNG.NextSingle(200, 800);

                circle.Scale = new Vector2(randomScale);
                circle.MoveTo(Vector2.Zero).MoveTo(randomVelocity * 80, randomSpeed + 400, Easing.OutQuint);
                circle.FadeIn().Delay(randomSpeed - 200).FadeOutFromOne(300);
            }
        }
    }
}
