using osu.Framework.Allocation;
using osu.Framework.Bindables;
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
        Background.Colour = Colour4.Transparent;
        Content.Padding = new MarginPadding(8);
        Content.Spacing = new Vector2(0, 8);
    }

    private partial class FluXisHueSelector : HueSelector
    {
        public FluXisHueSelector()
        {
            SliderBar.CornerRadius = 8;
            SliderBar.Masking = true;
        }

        protected override Drawable CreateSliderNub() => new SliderNub(Hue);

        private partial class SliderNub : CompositeDrawable
        {
            public SliderNub(Bindable<float> hue)
            {
                var box = new Box { RelativeSizeAxes = Axes.Both };

                InternalChild = new CircularContainer
                {
                    Height = 32,
                    Width = 12,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    BorderThickness = 4,
                    BorderColour = Theme.Text,
                    Child = box
                };

                hue.BindValueChanged(v => box.Colour = Colour4.FromHSL(v.NewValue, 1f, .5f), true);
            }
        }
    }

    private partial class FluXisSaturationSelector : SaturationValueSelector
    {
        public FluXisSaturationSelector()
        {
            CornerRadius = 12;
            Masking = true;
        }

        protected override Marker CreateMarker() => new FluXisMarker();

        private partial class FluXisMarker : Marker
        {
            public FluXisMarker()
            {
                Size = new Vector2(24);
                CornerRadius = Width / 2f;
                Masking = true;
                BorderThickness = 4;
                BorderColour = Theme.Text;

                InternalChild = new Box { RelativeSizeAxes = Axes.Both };

                Current.BindValueChanged(_ =>
                {
                    var bright = Theme.IsBright(Current.Value);

                    InternalChild.Colour = Current.Value;
                    BorderColour = bright ? Theme.Background2 : Theme.Text;
                }, true);
            }
        }
    }
}
