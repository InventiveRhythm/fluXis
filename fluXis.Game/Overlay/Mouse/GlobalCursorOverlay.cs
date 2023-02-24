using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Mouse;

public partial class GlobalCursorOverlay : Container
{
    private readonly Cursor cursor;

    public bool ShowCursor
    {
        set
        {
            if (value)
                this.FadeIn(200);
            else
                this.FadeOut(200);
        }
    }

    public GlobalCursorOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;

        Add(cursor = new Cursor());
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        cursor.Position = e.ScreenSpaceMousePosition;
        return base.OnMouseMove(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        cursor.Show();
        return false;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        cursor.Hide();
    }
}
