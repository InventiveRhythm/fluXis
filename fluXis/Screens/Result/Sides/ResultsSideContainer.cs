using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Result.Sides;

public abstract partial class ResultsSideContainer : CompositeDrawable
{
    protected abstract LocalisableString Title { get; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        EdgeEffect = FluXisStyles.ShadowMedium;
        CornerRadius = 16;
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
                Spacing = new Vector2(20),
                Padding = new MarginPadding(24),
                Direction = FillDirection.Vertical,
                Children = new[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.X,
                        Height = 16,
                        Child = new FluXisSpriteText
                        {
                            Text = Title,
                            WebFontSize = 20,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    },
                    CreateContent()
                }
            }
        };
    }

    protected abstract Drawable CreateContent();
}
