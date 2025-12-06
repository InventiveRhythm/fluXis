using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Outline;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using osu.Framework.Utils;

namespace fluXis.Input.Focus;

#nullable enable

public partial class FocusContainer : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    private IFocusable? current;
    private List<IFocusable> items => this.ChildrenOfType<IFocusable>(true).ToList();

    public Action<IFocusable>? ItemFocused { get; set; }
    public bool Active { get; set; } = true;

    protected override Container<Drawable> Content => content;
    private readonly Container content;

    public float OutlineWidth { get; set; } = 4;
    public float NonFocusPadding { get; set; } = 8;
    public float FocusPadding { get; set; } = 4;

    private readonly OutlinedSquare outline;
    private OutlineState outlineState;

    private bool holdingActivate;

    public FocusContainer()
    {
        InternalChildren = new Drawable[]
        {
            content = new Container { RelativeSizeAxes = Axes.Both },
            outline = new OutlinedSquare
            {
                Colour = Theme.Highlight,
                Alpha = 0
            }
        };
    }

    public void FocusFirst()
    {
        reset();
        switchActive(1);
    }

    private bool switchActive(int by)
    {
        if (items.Count == 0)
        {
            reset();
            return false;
        }

        if (current?.HoldingFocus ?? false)
            return true;

        var idx = current is null ? -1 : items.IndexOf(current);
        idx += by;

        if (idx < 0)
            idx = items.Count - 1;
        else if (idx > items.Count - 1)
            idx = 0;

        current = items[idx];
        ItemFocused?.Invoke(current);
        return true;
    }

    private void reset()
    {
        if (current?.HoldingFocus ?? false)
            current?.Deactivate();

        current = null;
    }

    protected override void Update()
    {
        base.Update();

        if (current != null && !items.Contains(current)) reset();

        if (current is null)
        {
            updateState(OutlineState.Hidden);
            return;
        }

        if (current is not null)
            updateState(current.HoldingFocus || holdingActivate ? OutlineState.Holding : OutlineState.Showing);

        if (current is null)
            return;

        var ssc = current.ScreenSpaceDrawQuad;
        var loc = ToLocalSpace(ssc);

        outline.BorderThickness = OutlineWidth;
        var pad = OutlineWidth + (current.HoldingFocus || holdingActivate ? FocusPadding : NonFocusPadding);
        var comp = current as CompositeDrawable;

        var rad = comp?.CornerRadius ?? 0;
        if (rad <= 0) rad = 4;

        outline.X = interp(outline.X, loc.TopLeft.X - pad);
        outline.Y = interp(outline.Y, loc.TopLeft.Y - pad);
        outline.Width = interp(outline.Width, loc.Width + pad * 2);
        outline.Height = interp(outline.Height, loc.Height + pad * 2);
        outline.CornerRadius = interp(outline.CornerRadius, rad);

        float interp(float cur, float tar)
        {
            if (Precision.AlmostEquals(tar, cur))
                return tar;

            return (float)Interpolation.Lerp(tar, cur, Math.Exp(-0.01 * Time.Elapsed));
        }
    }

    private void updateState(OutlineState state)
    {
        if (state == outlineState)
            return;

        outlineState = state;

        switch (state)
        {
            case OutlineState.Hidden:
                outline.ClearTransforms();
                outline.FadeOut(100);
                break;

            case OutlineState.Showing:
                outline.FadeTo(0.8f, 800).Then().FadeTo(0.4f, 800).Loop();
                break;

            case OutlineState.Holding:
                outline.ClearTransforms();
                outline.FadeIn(100);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (!Active)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.Previous:
                return switchActive(-1);

            case FluXisGlobalKeybind.Next:
                return switchActive(1);

            case FluXisGlobalKeybind.Select:
                var result = current?.Activate() ?? false;

                if (current != null && result && !current.HoldingFocus)
                    holdingActivate = true;

                return result;

            case FluXisGlobalKeybind.Back:
                if (current?.HoldingFocus ?? false)
                {
                    current.Deactivate();
                    return true;
                }

                break;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Select:
                if (holdingActivate) holdingActivate = false;
                break;
        }
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        reset();
        return base.OnMouseMove(e);
    }

    private enum OutlineState
    {
        Hidden,
        Showing,
        Holding
    }
}
