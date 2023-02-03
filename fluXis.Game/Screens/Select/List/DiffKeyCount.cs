using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.List
{
    public class DiffKeyCount : Container
    {
        public Colour4 KeyColour { get; set; }

        public DiffKeyCount(int count)
        {
            CornerRadius = 5;
            Masking = true;
            RelativeSizeAxes = Axes.Y;
            Width = 50;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;

            KeyColour = count switch
            {
                4 => Colour4.FromHex("62bafe"),
                5 => Colour4.FromHex("61f984"),
                6 => Colour4.FromHex("e3bb45"),
                7 => Colour4.FromHex("ec3b8d"),
                _ => Color4.White
            };

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KeyColour
                },
                new SpriteText
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Text = $"{count}K",
                    Font = new FontUsage("Quicksand", 20, "SemiBold"),
                    Margin = new MarginPadding { Right = 13 },
                    Y = -1,
                    Colour = KeyColour.ToHSL().Z > 0.5f ? Colour4.FromHex("#1a1a20") : Color4.White
                }
            };
        }
    }
}
