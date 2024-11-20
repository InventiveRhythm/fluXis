using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Blueprints.Selection;

public partial class SelectionBox : Container
{
    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    [Resolved]
    private ITimePositionProvider positionProvider { get; set; }

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

    public Container Box { get; private set; }
    private Box box;

    private bool horizontal { get; }
    private double? startTime;

    public SelectionBox(bool horizontal = false)
    {
        this.horizontal = horizontal;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChild = Box = new Container
        {
            BorderColour = FluXisColors.Text,
            BorderThickness = 4,
            CornerRadius = 10,
            Masking = true,
            Child = box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.2f
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        beatSync.OnBeat += _ =>
        {
            box.FadeTo(.3f)
               .FadeTo(.2f, beatSync.BeatTime);
        };
    }

    public void HandleDrag(MouseButtonEvent e)
    {
        Box.Position = Vector2.ComponentMin(e.MouseDownPosition, e.MousePosition);
        Box.Size = Vector2.ComponentMax(e.MouseDownPosition, e.MousePosition) - Box.Position;

        startTime ??= positionProvider.TimeAtScreenSpacePosition(e.ScreenSpaceMouseDownPosition);
        var end = positionProvider.TimeAtScreenSpacePosition(e.ScreenSpaceMousePosition);

        var startPos = ToLocalSpace(positionProvider.ScreenSpacePositionAtTime(startTime.Value, 0));
        var endPos = ToLocalSpace(positionProvider.ScreenSpacePositionAtTime(end, 0));

        if (horizontal)
        {
            Box.X = Math.Min(startPos.X, endPos.X);
            Box.Width = Math.Max(startPos.X, endPos.X) - Box.X;
        }
        else
        {
            Box.Y = Math.Min(startPos.Y, endPos.Y);
            Box.Height = Math.Max(startPos.Y, endPos.Y) - Box.Y;
        }
    }

    public override void Show() => State = Visibility.Visible;

    public override void Hide()
    {
        State = Visibility.Hidden;
        startTime = null;
    }
}
