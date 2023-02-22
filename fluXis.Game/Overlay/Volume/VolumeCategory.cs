using fluXis.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Volume;

public partial class VolumeCategory : Container
{
    public VolumeCategory(string type, BindableNumber<double> bind)
    {
        AutoSizeAxes = Axes.Both;
        CornerRadius = 5;
        Masking = true;

        SpriteText percentageText;
        Box progress;

        AddRangeInternal(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10, 0),
                Padding = new MarginPadding { Horizontal = 10, Bottom = 5 },
                Children = new[]
                {
                    new SpriteText
                    {
                        Text = type,
                        Font = new FontUsage("Quicksand", 48, "Bold")
                    },
                    percentageText = new SpriteText
                    {
                        Text = (int)(bind.Value * 100) + "%",
                        Font = new FontUsage("Quicksand", 48)
                    }
                }
            },
            progress = new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Width = (float)bind.Value,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Colour = Colour4.White,
            }
        });

        bind.BindValueChanged(e =>
        {
            percentageText.Text = (int)(e.NewValue * 100) + "%";
            progress.Width = (float)e.NewValue;
        });
    }
}
