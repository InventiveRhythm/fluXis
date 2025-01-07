using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Graphics.UserInterface.Color;

public partial class FluXisHsvColourPicker : HSVColourPicker
{
    protected override HueSelector CreateHueSelector() => new FluXisHueSelector();
    protected override SaturationValueSelector CreateSaturationValueSelector() => new FluXisSaturationSelector();

    [BackgroundDependencyLoader]
    private void load()
    {
        Background.Colour = FluXisColors.Background4;
        Content.Padding = new MarginPadding(10);
        Content.Spacing = new Vector2(0, 10);
    }

    private partial class FluXisHueSelector : HueSelector
    {
        public FluXisHueSelector()
        {
            SliderBar.CornerRadius = 10;
            SliderBar.Masking = true;
        }

        protected override Drawable CreateSliderNub() => new SliderNub();

        private partial class SliderNub : CompositeDrawable
        {
            public SliderNub()
            {
                InternalChild = new CircularContainer
                {
                    Height = 35,
                    Width = 10,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    BorderThickness = 3,
                    BorderColour = FluXisColors.Text,
                    Child = new Box { RelativeSizeAxes = Axes.Both }
                };
            }
        }
    }

    private partial class FluXisSaturationSelector : SaturationValueSelector
    {
        public FluXisSaturationSelector()
        {
            CornerRadius = 10;
            Masking = true;
        }

        protected override Marker CreateMarker() => new FluXisMarker();

        private partial class FluXisMarker : Marker
        {
            public FluXisMarker()
            {
                Size = new Vector2(20);
                CornerRadius = 10;
                Masking = true;
                BorderThickness = 3;
                BorderColour = FluXisColors.Text;

                InternalChild = new Box { RelativeSizeAxes = Axes.Both };

                Current.BindValueChanged(_ => InternalChild.Colour = Current.Value, true);
            }
        }
    }
}
