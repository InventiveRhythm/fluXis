using System;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class JudgementDisplay : GameplayHUDElement
    {
        public JudgementDisplay(GameplayScreen screen)
            : base(screen)
        {
        }

        private SpriteText text;
        private Bindable<bool> hideFlawless;

        [BackgroundDependencyLoader]
        private void load(FluXisConfig config)
        {
            hideFlawless = config.GetBindable<bool>(FluXisSetting.HideFlawless);

            Anchor = Anchor.TopCentre;
            Origin = Anchor.Centre;
            RelativePositionAxes = Axes.Y;
            Y = 0.6f;

            InternalChild = text = new SpriteText
            {
                Font = new FontUsage("Quicksand", 48f, "SemiBold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Blending = BlendingParameters.Additive
            };
        }

        public void PopUp(HitWindow hitWindow)
        {
            if (hideFlawless.Value && hitWindow.Key == Judgements.Flawless) return;

            const int random_angle = 7;
            float scale = 1.4f;
            float rotation = 0;

            if (hitWindow.Key == Judgements.Miss)
            {
                scale = 1.8f;
                rotation = new Random().Next(-random_angle, random_angle);
            }

            text.Text = hitWindow.Key.ToString();
            text.Colour = hitWindow.Color;
            text.RotateTo(rotation)
                .ScaleTo(1f)
                .TransformSpacingTo(new Vector2(0, 0))
                .ScaleTo(scale, 1000, Easing.OutQuint)
                .FadeOutFromOne(500)
                .TransformSpacingTo(new Vector2(5, 0), 1000, Easing.OutQuint);
        }
    }
}
