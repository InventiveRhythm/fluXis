using fluXis.Game.Graphics.Sprites;
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
    private IHasDrawableTooltip lastHoveredDrawable;

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
        bool foundHovered = false;

        foreach (var drawable in inputManager.HoveredDrawables)
        {
            switch (drawable)
            {
                case IHasTextTooltip desc:
                {
                    var newTip = desc.Tooltip ?? "";

                    if (newTip != tooltipText)
                    {
                        tooltipText = newTip;
                        cursor.DrawableTooltip = new FluXisSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Margin = new MarginPadding { Horizontal = 5, Vertical = 2 },
                            Shadow = true,
                            Text = desc.Tooltip
                        };
                    }

                    foundHovered = true;

                    break;
                }

                case IHasDrawableTooltip drawTooltip:
                {
                    if (drawTooltip != lastHoveredDrawable)
                    {
                        lastHoveredDrawable = drawTooltip;
                        cursor.DrawableTooltip = drawTooltip.GetTooltip();
                    }

                    foundHovered = true;

                    break;
                }
            }
        }

        if (!foundHovered)
        {
            tooltipText = null;
            lastHoveredDrawable = null;
            cursor.DrawableTooltip = null;
        }

        timeInactive += Time.Elapsed;

        switch (timeInactive)
        {
            case > 4000 when !isHidden:
                cursor.FadeOut(1200);
                isHidden = true;
                break;

            case < 4000 when isHidden:
                cursor.FadeIn(200);
                isHidden = false;
                break;
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
