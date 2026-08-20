using System;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Preview;

public partial class ChartingPreviewContainer : CompositeDrawable
{
    private Box dim;
    private ClickableContainer wrapper;
    private DraggableContainer dragContainer;
    private bool full;

    [UsedImplicitly]
    // ReSharper disable once RedundantDefaultMemberInitializer
    private float fullProgress = 0f;

    [UsedImplicitly]
    // ReSharper disable once RedundantDefaultMemberInitializer
    private float dimProgress = 0f;

    [UsedImplicitly]
    // ReSharper disable once RedundantDefaultMemberInitializer
    private float resizeProgress = 0f;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren =
        [
            dim = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0f
            },
            dragContainer = new DraggableContainer
            {
                DraggableArea = 6,
                Padding = new MarginPadding(12),
                Child = wrapper = new ClickableContainer
                {
                    Action = toggle,
                    Child = new AspectRatioContainer(true)
                    {
                        CornerRadius = 12,
                        Masking = true,
                        Child = new ChartingPreview()
                    }
                }
            }
        ];

        dragContainer.OnMouseDownAction = e =>
        {
            if (e.Button != MouseButton.Right) return;

            resizeProgress = 0f;
            this.TransformTo(nameof(fullProgress), resizeProgress, Styling.TRANSITION_MOVE, Easing.OutQuint);
            if (full) toggle();
        };
        dragContainer.OnDraggingStart = _ => dragContainer.ClearTransforms(targetMember: nameof(dragContainer.Position));
        dragContainer.OnDraggingEnd = _ =>
        {
            var screenCenter = dragContainer.ToScreenSpace(dragContainer.DrawRectangle.Centre);
            var localCenter = Parent!.ToLocalSpace(screenCenter);

            bool isLeft = localCenter.X < Parent.DrawWidth / 2f;
            bool isTop = localCenter.Y < Parent.DrawHeight / 2f;

            Anchor targetAnchor = (isLeft ? Anchor.x0 : Anchor.x2) | (isTop ? Anchor.y0 : Anchor.y2);

            Vector2 targetCornerLocal = new Vector2(
                isLeft ? 0 : dragContainer.DrawWidth,
                isTop ? 0 : dragContainer.DrawHeight
            );

            Vector2 parentAnchorLocal = new Vector2(
                isLeft ? 0 : Parent.DrawWidth,
                isTop ? 0 : Parent.DrawHeight
            );

            Vector2 currentCornerParentLocal = Parent.ToLocalSpace(dragContainer.ToScreenSpace(targetCornerLocal));

            dragContainer.Anchor = targetAnchor;
            dragContainer.Origin = targetAnchor;

            dragContainer.Position = currentCornerParentLocal - parentAnchorLocal;

            dragContainer.MoveTo(Vector2.Zero, 500, Easing.OutElasticQuarter);
        };
    }

    protected override void Update()
    {
        base.Update();
        updateSizeAndPosition();

        return;

        void updateSizeAndPosition()
        {
            var padding = new Vector2(dragContainer.Padding.TotalHorizontal, dragContainer.Padding.TotalVertical);
            var min = new Vector2(384, 216) + padding;
            var max = Parent!.DrawSize - padding;

            var delta = max - min;

            var dragDelta = dragContainer.DragDelta;

            if (dragDelta != Vector2.Zero)
            {
                dragContainer.DragDelta = Vector2.Zero;

                switch (full)
                {
                    case false when delta is { X: > 0, Y: > 0 } && dragContainer.IsResizing:
                    {
                        ClearTransforms(true, nameof(fullProgress));

                        float dirX = dragContainer.Anchor.HasFlag(Anchor.x2) ? -1f : 1f;
                        float dirY = dragContainer.Anchor.HasFlag(Anchor.y2) ? -1f : 1f;

                        var progressDelta = ((dragDelta.X * dirX) / delta.X + (dragDelta.Y * dirY) / delta.Y) / 2f;
                        resizeProgress = progressDelta;
                        fullProgress = Math.Clamp(fullProgress + resizeProgress, 0f, 0.5f);
                        break;
                    }

                    case false when delta is { X: > 0, Y: > 0 } && !dragContainer.IsResizing:
                        dragContainer.Position += dragDelta;
                        break;
                }
            }

            if (delta.X <= 0 || delta.Y <= 0)
                return;

            wrapper.Size = min + delta * fullProgress;
            dim.Alpha = dimProgress * 0.9f;
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.F && !e.Repeat)
        {
            toggle();
            return true;
        }

        return base.OnKeyDown(e);
    }

    private void toggle()
    {
        ClearTransforms(true, nameof(fullProgress));

        if (!full)
        {
            resizeProgress = fullProgress;
            this.TransformTo(nameof(fullProgress), 1f, Styling.TRANSITION_MOVE, Easing.OutQuint);
        }
        else
        {
            this.TransformTo(nameof(fullProgress), resizeProgress, Styling.TRANSITION_MOVE, Easing.OutQuint);
        }

        full = !full;
        this.TransformTo(nameof(dimProgress), full ? 1f : 0f, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }
}
