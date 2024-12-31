using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Overlay.Volume;

public partial class VolumeCategory : CompositeDrawable
{
    private string text { get; }
    public Bindable<double> Bindable { get; }
    private VolumeOverlay volumeOverlay { get; }

    private FluXisSpriteText percentText;
    private Box background;

    public VolumeCategory(string text, Bindable<double> bindable, VolumeOverlay volumeOverlay)
    {
        this.text = text;
        Bindable = bindable;
        this.volumeOverlay = volumeOverlay;
    }

    [BackgroundDependencyLoader]
    private void load()
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
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Padding = new MarginPadding(5),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = text,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            percentText = new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
                            },
                        }
                    },
                    new FluXisSlider<double>
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 10,
                        Bindable = Bindable,
                        Step = 0.01f,
                        PlaySample = false,
                        ShowArrowButtons = false
                    },
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Bindable.BindValueChanged(updateProgress, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= updateProgress;
    }

    private void updateProgress(ValueChangedEvent<double> e)
    {
        percentText.Text = $"{e.NewValue:P0}";
    }

    public void UpdateSelected(bool newValue)
    {
        background.FadeTo(newValue ? 0.1f : 0, 100);
    }

    protected override bool OnHover(HoverEvent e)
    {
        volumeOverlay.SelectCategory(this);
        return true;
    }
}
