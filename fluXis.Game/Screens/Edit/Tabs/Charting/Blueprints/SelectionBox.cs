using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class SelectionBox : Container
{
    public Container Box { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChild = Box = new Container
        {
            BorderColour = Colour4.White,
            BorderThickness = 4,
            CornerRadius = 10,
            Masking = true,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.2f
            }
        };
    }

    public void HandleDrag(MouseButtonEvent e)
    {
        Box.Position = Vector2.ComponentMin(e.MouseDownPosition, e.MousePosition);
        Box.Size = Vector2.ComponentMax(e.MouseDownPosition, e.MousePosition) - Box.Position;
    }

    private Visibility state;

    public Visibility State
    {
        get => state;
        set
        {
            if (value == state) return;

            state = value;
            this.FadeTo(state == Visibility.Hidden ? 0 : 1, 250, Easing.OutQuint);
        }
    }

    public override void Hide() => State = Visibility.Hidden;

    public override void Show() => State = Visibility.Visible;
}
