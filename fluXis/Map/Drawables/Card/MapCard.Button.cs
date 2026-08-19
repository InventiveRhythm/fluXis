using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Map.Drawables.Card;

public partial class MapCard
{
    private partial class Button : CompositeDrawable
    {
        public IconUsage Icon { set => icon.Icon = value; }

        public new Colour4 Colour
        {
            set
            {
                hover.Colour = value;
                flash.Colour = value;
                icon.Colour = value;
            }
        }

        public Func<bool> Action { get; set; } = () => false;

        [Resolved]
        private UISamples samples { get; set; }

        private readonly HoverLayer hover;
        private readonly FlashLayer flash;
        private readonly FluXisSpriteIcon icon;

        public Button()
        {
            RelativeSizeAxes = Axes.X;
            Height = Expand.BUTTON_HEIGHT;

            InternalChildren =
            [
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                icon = new FluXisSpriteIcon
                {
                    Icon = Phosphor.Bold.Question,
                    Size = new Vector2(14),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            ];
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.Show();
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();
            samples.Click();
            return Action();
        }
    }
}
