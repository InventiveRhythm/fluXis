using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Overlay.Mouse;

public partial class GlobalCursorOverlay : Container
{
    public Vector2 RelativePosition => new(cursor.Position.X / DrawWidth, cursor.Position.Y / DrawHeight);

    private readonly Cursor cursor;

    private InputManager inputManager;
    private double timeInactive;
    private bool isHidden;
    private LocalisableString tooltipText;
    private IHasDrawableTooltip lastHoveredDrawable;

    // tracking stuff when hidden
    private Vector2 firstMove;
    private double firstMoveTime;

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
        cursor.Position = ToLocalSpace(inputManager.CurrentState.Mouse.Position);

        bool foundHovered = false;

        foreach (var drawable in inputManager.HoveredDrawables)
        {
            switch (drawable)
            {
                case IHasTextTooltip desc:
                {
                    var newTip = desc.Tooltip;

                    if (newTip != tooltipText)
                    {
                        tooltipText = newTip;
                        cursor.DrawableTooltip = new FluXisSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Margin = new MarginPadding { Horizontal = 10, Vertical = 6 },
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
            tooltipText = "";
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
        if (!isHidden) return false;

        if (firstMoveTime == 0 || Time.Current - firstMoveTime > 1000)
        {
            firstMove = e.MousePosition;
            firstMoveTime = Time.Current;
        }

        if (Vector2Extensions.Distance(firstMove, e.MousePosition) < 10) return false;

        timeInactive = 0;
        firstMoveTime = 0;

        return false;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        cursor.ShowOutline();
        return false;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        cursor.HideOutline();
    }
}
