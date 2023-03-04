using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Mouse;

public partial class GlobalCursorOverlay : Container
{
    private readonly Cursor cursor;

    private InputManager inputManager;

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

        Add(cursor = new Cursor(this));
    }

    protected override void LoadComplete()
    {
        inputManager = GetContainingInputManager();
        base.LoadComplete();
    }

    protected override void Update()
    {
        IHasTooltip tooltip = null;

        foreach (var drawable in inputManager.HoveredDrawables)
        {
            if (drawable is IHasTooltip desc)
            {
                tooltip = desc;
                break;
            }
        }

        cursor.TooltipText = tooltip?.Tooltip ?? "";

        base.Update();
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        cursor.Position = e.MousePosition;
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
