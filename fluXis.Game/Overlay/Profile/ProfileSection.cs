using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Profile;

public partial class ProfileSection : Container
{
    public virtual string Title { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(10),
                Children = new[]
                {
                    new SpriteText
                    {
                        Text = Title,
                        Font = FluXisFont.Default(45),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    CreateContent()
                }
            }
        };
    }

    public virtual Drawable CreateContent()
    {
        return new SpriteText
        {
            Text = "Nothing here yet...",
            Font = FluXisFont.Default(23),
            Colour = FluXisColors.Text2,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre
        };
    }
}
