using fluXis.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Volume;

public partial class VolumeCategory : Container
{
    public VolumeOverlay VolumeOverlay { get; set; }

    public string Text
    {
        get => text.Text.ToString();
        set => text.Text = value;
    }

    public Bindable<double> Bindable
    {
        get => bindable;
        set
        {
            bindable = value;
            bindable.BindValueChanged(v => updateProgress(v.NewValue), true);
        }
    }

    private readonly FluXisSpriteText text;
    private readonly FluXisSpriteText percentText;
    private readonly Box background;
    private readonly Box progressBackground;
    private readonly Box progress;

    private Bindable<double> bindable;

    public VolumeCategory()
    {
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;
        CornerRadius = 5;
        Masking = true;

        AddRangeInternal(new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new Container
            {
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Padding = new MarginPadding(5),
                Children = new Drawable[]
                {
                    text = new FluXisSpriteText(),
                    percentText = new FluXisSpriteText
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight
                    },
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 10,
                        Masking = true,
                        Margin = new MarginPadding { Top = 20 },
                        Children = new Drawable[]
                        {
                            progressBackground = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = .5f
                            },
                            progress = new Box
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    }
                }
            }
        });
    }

    private void updateProgress(double value)
    {
        progress.ResizeWidthTo((float)value, 100, Easing.OutQuint);
        percentText.Text = $"{value:P0}";

        var color = FluXisColors.AccentGradient.Interpolate(new Vector2((float)value));
        progress.FadeColour(color, 100);
        progressBackground.FadeColour(color, 100);
    }

    public void UpdateSelected(bool newValue)
    {
        background.FadeTo(newValue ? 0.1f : 0, 100);
    }

    protected override bool OnHover(HoverEvent e)
    {
        VolumeOverlay.SelectCategory(this);
        return true;
    }
}
