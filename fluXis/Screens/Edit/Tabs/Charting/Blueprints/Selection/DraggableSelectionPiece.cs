using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class DraggableSelectionPiece : Container
{
    public Action<Vector2> DragAction { get; init; }

    private readonly BlueprintNotePiece piece;

    public DraggableSelectionPiece()
    {
        RelativeSizeAxes = Axes.X;
        Height = 42;

        Child = piece = new BlueprintNotePiece
        {
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = .5f
        };
    }

    protected override void Update()
    {
        var factor = DrawWidth / 114f;
        Height = 42f * factor;
    }

    protected override bool OnDragStart(DragStartEvent e) => e.Button == MouseButton.Left;

    protected override void OnDrag(DragEvent e)
    {
        DragAction?.Invoke(e.ScreenSpaceMousePosition);
        base.OnDrag(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        piece.MouseDown();
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        piece.MouseUp();
    }
}
