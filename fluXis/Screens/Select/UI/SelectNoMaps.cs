using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Localization;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Select.UI;

public partial class SelectNoMaps : CompositeDrawable
{
    private AudioFilter lowpass;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        CornerRadius = 20;
        Masking = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            lowpass = new AudioFilter(audio.TrackMixer),
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(20),
                Spacing = new Vector2(-2),
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Margin = new MarginPadding { Bottom = 8 },
                        Icon = FontAwesome6.Solid.TriangleExclamation,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(32)
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.SongSelect.NoMapsFound,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 20
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.SongSelect.NoMapsFoundDescription,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 14,
                        Alpha = .8f
                    }
                }
            }
        };
    }

    public override void Show()
    {
        lowpass.CutoffTo(AudioFilter.MIN, 600, Easing.OutQuint);
        this.FadeIn(200).ScaleTo(.8f).ScaleTo(1, 800, Easing.OutElasticHalf);
    }

    public override void Hide()
    {
        lowpass.CutoffTo(AudioFilter.MAX, 600, Easing.OutQuint);
        this.FadeOut(200).ScaleTo(0.9f, 400, Easing.OutQuint);
    }
}
