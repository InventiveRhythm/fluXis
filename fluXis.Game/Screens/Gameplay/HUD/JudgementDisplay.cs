using System;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class JudgementDisplay : GameplayHUDElement
    {
        public JudgementDisplay(GameplayScreen screen)
            : base(screen)
        {
        }

        private SpriteText text;

        [BackgroundDependencyLoader]
        private void load()
        {
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

        public void PopUp(Judgement judgement)
        {
            const int random_angle = 7;
            float scale = 1.4f;
            float rotation = 0;

            if (judgement.Key == Judgements.Miss)
            {
                scale = 1.8f;
                rotation = new Random().Next(-random_angle, random_angle);
            }

            text.Text = judgement.Key.ToString();
            text.Colour = judgement.Color;
            text.RotateTo(rotation).ScaleTo(1f).ScaleTo(scale, 1000, Easing.OutBack).FadeOutFromOne(500);
        }
    }
}
