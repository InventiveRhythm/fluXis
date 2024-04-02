using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Integration;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Bases.Judgements;
using fluXis.Shared.Scoring.Structs;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class JudgementDisplay : GameplayHUDComponent
{
    [Resolved]
    private LightController lightController { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    private AbstractJudgementText missGhost;
    private Container skinnableTextContainer;

    private Bindable<bool> hideFlawless;
    private Bindable<bool> showEarlyLate;
    private Bindable<bool> judgementSplash;

    private Splash splash;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        hideFlawless = config.GetBindable<bool>(FluXisSetting.HideFlawless);
        showEarlyLate = config.GetBindable<bool>(FluXisSetting.ShowEarlyLate);
        judgementSplash = config.GetBindable<bool>(FluXisSetting.JudgementSplash);

        Screen.JudgementProcessor.ResultAdded += popUp;

        InternalChildren = new Drawable[]
        {
            splash = new Splash(),
            missGhost = skinManager.GetJudgement(Shared.Scoring.Enums.Judgement.Miss, false).With(d =>
            {
                d.Alpha = 0.5f;
                d.Scale = new Vector2(1.4f);
                d.Anchor = Anchor.Centre;
                d.Origin = Anchor.Centre;
            }),
            skinnableTextContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    private void popUp(HitResult result)
    {
        var judgement = result.Judgement;

        if (hideFlawless.Value && judgement == Shared.Scoring.Enums.Judgement.Flawless)
            return;

        skinnableTextContainer.RemoveAll(_ => true, true);

        var judgementText = skinManager.GetJudgement(judgement, result.Difference < 0);
        skinnableTextContainer.Add(judgementText);
        judgementText.Animate(showEarlyLate.Value);

        if (judgement == Shared.Scoring.Enums.Judgement.Miss)
            missGhost.Animate(false);

        if (judgementSplash.Value)
            doSplash(judgement);

        lightController.FadeColour(skinManager.SkinJson.GetColorForJudgement(judgement))
                       .FadeColour(Color4.Black, 400);
    }

    private void doSplash(Shared.Scoring.Enums.Judgement judgement)
    {
        splash.Colour = skinManager.SkinJson.GetColorForJudgement(judgement);
        splash.Splat(judgement);
    }

    private partial class Splash : CompositeDrawable
    {
        private CircularContainer circle;
        private Container<Circle> dots;

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                circle = new CircularContainer
                {
                    Size = new Vector2(100),
                    Scale = Vector2.Zero,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    BorderColour = Color4.White,
                    BorderThickness = 5,
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        AlwaysPresent = true,
                        Alpha = 0
                    }
                },
                dots = new Container<Circle>
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    ChildrenEnumerable = Enumerable.Range(0, 8).Select(_ => new Circle
                    {
                        Size = new Vector2(20),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0
                    })
                }
            };
        }

        public void Splat(Shared.Scoring.Enums.Judgement judgement)
        {
            const float base_travel = 80;

            var count = 1;
            var travel = base_travel;

            switch (judgement)
            {
                case Shared.Scoring.Enums.Judgement.Flawless:
                    count = 8;
                    travel *= 1f;
                    break;

                case Shared.Scoring.Enums.Judgement.Perfect:
                case Shared.Scoring.Enums.Judgement.Great:
                    count = 4;
                    travel *= 0.8f;
                    break;

                case Shared.Scoring.Enums.Judgement.Alright:
                case Shared.Scoring.Enums.Judgement.Okay:
                    count = 2;
                    travel *= 0.4f;
                    break;

                case Shared.Scoring.Enums.Judgement.Miss:
                    count = 0;
                    travel *= 1.2f;
                    break;

                case Shared.Scoring.Enums.Judgement.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(judgement), judgement, null);
            }

            circle.TransformTo(nameof(BorderThickness), 20f)
                  .TransformTo(nameof(BorderThickness), 0f, 1000, Easing.OutQuint)
                  .ScaleTo(0f).ScaleTo(1.4f * (travel / base_travel), 600, Easing.OutQuint);

            var idx = 0;

            foreach (var dot in dots)
            {
                if (idx++ >= count)
                {
                    dot.FadeOut();
                    continue;
                }

                var direction = RNG.NextSingle(0, 360);
                var distance = RNG.NextSingle(travel / 2, travel);
                var randomScale = RNG.NextSingle(0.5f, 1f);

                var vec = new Vector2(
                    MathF.Cos(direction) * distance,
                    MathF.Sin(direction) * distance
                );

                dot.Scale = new Vector2(randomScale);
                dot.MoveTo(Vector2.Zero)
                   .MoveTo(vec, 800, Easing.OutCirc)
                   .FadeIn().Delay(200).FadeOut(400);
            }
        }
    }
}
