using fluXis.Audio;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface;

public partial class FluXisToggleSwitch : Container
{
    public Bindable<bool> State { get; init; } = new();

    [Resolved]
    private UISamples samples { get; set; }

    private bool initial = true;

    private Box background;
    private Circle nubHover;
    private Circle nub;

    private Vector2 dragStartPos;

    private Sample toggleOn;
    private Sample toggleOff;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        toggleOn = samples.Get(@"UI/toggle-on");
        toggleOff = samples.Get(@"UI/toggle-off");

        Size = new Vector2(80, 32);

        InternalChildren = new Drawable[]
        {
            new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background3
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 16 },
                Children = new Drawable[]
                {
                    nubHover = new Circle
                    {
                        RelativePositionAxes = Axes.X,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.Centre,
                        Size = new Vector2(16),
                        Alpha = .2f
                    },
                    nub = new Circle
                    {
                        RelativePositionAxes = Axes.X,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.Centre,
                        Size = new Vector2(16),
                        Colour = FluXisColors.Background5
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        State.BindValueChanged(v =>
        {
            background.FadeColour(v.NewValue ? FluXisColors.Highlight : FluXisColors.Background3, 200);
            nub.FadeColour(v.NewValue ? FluXisColors.Background3 : FluXisColors.Background5, 200);
            nub.MoveToX(v.NewValue ? 1 : 0, 400, Easing.OutQuint);
            nub.ResizeTo(v.NewValue ? 24 : 16, 400, Easing.OutQuint);

            if (initial)
                return;

            if (v.NewValue)
                toggleOn?.Play();
            else
                toggleOff?.Play();
        }, true);

        initial = false;
    }

    protected override void Update()
    {
        base.Update();

        nubHover.X = nub.X;
        nubHover.Colour = nub.Colour;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        dragStartPos = e.MousePosition;
        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        var diff = e.MousePosition.X - dragStartPos.X;

        State.Value = diff switch
        {
            > 5 => true,
            < 5 => false,
            _ => State.Value
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        nubHover.ResizeTo(38, 50, Easing.Out);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        nubHover.ResizeTo(16, 200, Easing.Out);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        nub.ResizeTo(28, 100, Easing.Out);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        nub.ResizeTo(State.Value ? 24 : 16, 100, Easing.Out);
    }

    protected override bool OnClick(ClickEvent e)
    {
        State.Value = !State.Value;
        return base.OnClick(e);
    }
}
