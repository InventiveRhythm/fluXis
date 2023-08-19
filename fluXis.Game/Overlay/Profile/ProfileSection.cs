using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

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
                Colour = FluXisColors.Background2
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(10),
                Children = new[]
                {
                    new FluXisSpriteText
                    {
                        Text = Title,
                        FontSize = 45,
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
        return new FluXisSpriteText
        {
            Text = "Nothing here yet...",
            FontSize = 24,
            Colour = FluXisColors.Text2,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre
        };
    }
}
