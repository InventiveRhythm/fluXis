using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Mouse;

public partial class GlobalCursorOverlay : Container
{
    private readonly Cursor cursor;

    private InputManager inputManager;
    private double timeInactive;
    private bool isHidden;
    private string tooltipText;

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

        var newTip = tooltip?.Tooltip ?? "";

        if (newTip != tooltipText)
        {
            tooltipText = newTip;
            cursor.TooltipText = tooltipText;
        }

        timeInactive += Time.Elapsed;

        if (timeInactive > 4000 && !isHidden)
        {
            cursor.FadeOut(1200);
            isHidden = true;
        }
        else if (timeInactive < 5000 && isHidden)
        {
            cursor.FadeIn(200);
            isHidden = false;
        }
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        cursor.Position = e.MousePosition;
        timeInactive = 0;
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
