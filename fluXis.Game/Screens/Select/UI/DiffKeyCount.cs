using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.UI
{
    public class DiffKeyCount : Container
    {
        public DiffKeyCount(int count)
        {
            CornerRadius = 5;
            Masking = true;
            AutoSizeAxes = Axes.Both;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Margin = new MarginPadding { Right = 10 };

            Colour4 colour = Color4.White;

            switch (count)
            {
                case 4:
                    colour = Colour4.FromHex("62bafe");
                    break;

                case 5:
                    colour = Colour4.FromHex("61f984");
                    break;

                case 6:
                    colour = Colour4.FromHex("e3bb45");
                    break;

                case 7:
                    colour = Colour4.FromHex("ec3b8d");
                    break;
            }

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colour
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = $"{count}K",
                    Font = new FontUsage("Quicksand", 20, "SemiBold"),
                    Margin = new MarginPadding { Horizontal = 10 },
                    Y = -1
                }
            };
        }
    }
}
