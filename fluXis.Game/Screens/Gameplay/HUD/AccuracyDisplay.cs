using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Gameplay.HUD
{
    public class AccuracyDisplay : GameplayHUDElement
    {
        private SpriteText accuracyText;
        private float displayedAccuracy;
        private float lastAccuracy;

        public AccuracyDisplay(GameplayScreen screen)
            : base(screen)
        {
            displayedAccuracy = 0;
            lastAccuracy = 0;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.TopCentre;

            Add(accuracyText = new SpriteText
            {
                Font = new FontUsage("Quicksand", 32f, "SemiBold", false, true),
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre
            });
        }

        protected override void LoadComplete()
        {
            accuracyText.FadeOut().Then(1000).FadeInFromZero(800);

            base.LoadComplete();
        }

        protected override void Update()
        {
            var currentAccuracy = Screen.Performance.Accuracy;

            if (currentAccuracy != lastAccuracy)
            {
                this.TransformTo(nameof(displayedAccuracy), currentAccuracy, 400, Easing.Out);
                lastAccuracy = currentAccuracy;
            }

            accuracyText.Text = $"{displayedAccuracy:00.00}%".Replace(",", ".");

            base.Update();
        }
    }
}
