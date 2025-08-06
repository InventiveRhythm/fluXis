using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Volume;

public partial class VolumeCategory : CompositeDrawable
{
    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    private string text { get; }
    public Bindable<double> Bindable { get; }
    private VolumeOverlay volumeOverlay { get; }

    private double smoothProgress;
    private CircularProgress progressBackground;
    private CircularProgress progress;
    private FluXisSpriteText percentText;

    public VolumeCategory(string text, Bindable<double> bindable, VolumeOverlay volumeOverlay)
    {
        this.text = text;
        Bindable = bindable;
        this.volumeOverlay = volumeOverlay;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Size = new Vector2(196);

        InternalChildren = new Drawable[]
        {
            progressBackground = new CircularProgress
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Text,
                RoundedCaps = true,
                Progress = .8f,
                InnerRadius = .1f,
                Rotation = 180 + 360 * .1f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = .2f,
                Scale = new Vector2(.95f)
            },
            progress = new CircularProgress
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Text,
                RoundedCaps = true,
                Progress = Bindable.Value,
                InnerRadius = .2f,
                Rotation = 180 + 360 * .1f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(-4),
                Children = new Drawable[]
                {
                    percentText = new FluXisSpriteText
                    {
                        WebFontSize = 24,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        WebFontSize = 16,
                        Text = text,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Alpha = .6f
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        smoothProgress = Bindable.Value;
        Bindable.BindValueChanged(updateProgress, true);

        beatSync.OnBeat += _ =>
        {
            progressBackground.ScaleTo(.96f).FadeTo(.25f).TransformTo(nameof(CircularProgress.InnerRadius), 0.12f)
                              .ScaleTo(.95f, beatSync.BeatTime).FadeTo(.2f, beatSync.BeatTime)
                              .TransformTo(nameof(CircularProgress.InnerRadius), 0.1f, beatSync.BeatTime);
        };
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Bindable.ValueChanged -= updateProgress;
    }

    protected override void Update()
    {
        base.Update();

        percentText.Text = $"{smoothProgress:P0}";
        progress.Progress = smoothProgress * 0.8f;
    }

    private void updateProgress(ValueChangedEvent<double> e)
    {
        this.TransformTo(nameof(smoothProgress), e.NewValue, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }

    public void UpdateSelected(bool newValue)
    {
        this.ScaleTo(newValue ? 1f : .75f, Styling.TRANSITION_MOVE, Easing.OutQuint)
            .FadeTo(newValue ? 1f : .8f, Styling.TRANSITION_FADE);
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        volumeOverlay.ResetInactiveTimer();
        Bindable.Value += e.ScrollDelta.Y > 0 ? .02f : -.02;
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (volumeOverlay.Alpha < 0.001f)
            return false;

        volumeOverlay.SelectCategory(this);
        return true;
    }
}
