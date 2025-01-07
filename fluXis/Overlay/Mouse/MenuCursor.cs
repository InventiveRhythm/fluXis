using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Overlay.Mouse;

public partial class MenuCursor : CursorContainer
{
    private DrawableCursor cursor;

    public Vector2 CursorPosition => cursor.Position;

    private InputManager inputManager;
    private Vector2? mouseDownPosition;
    private bool rotating;

    protected override Drawable CreateCursor() => cursor = new DrawableCursor();

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }

    protected override void Update()
    {
        if (mouseDownPosition != null && inputManager.DraggedDrawable is ScrollContainer<Drawable> or ICursorDrag)
        {
            var mouseDelta = inputManager.CurrentState.Mouse.Position - mouseDownPosition.Value;

            if (!rotating)
                rotating = mouseDelta.LengthSquared > 20;

            if (rotating)
            {
                var angle = float.RadiansToDegrees(MathF.Atan2(-mouseDelta.X, mouseDelta.Y)) + 24.3f;

                float delta = (angle - cursor.Rotation) % 360;
                if (delta < -180) delta += 360;
                if (delta > 180) delta -= 360;
                angle = cursor.Rotation + delta;

                cursor.RotateTo(angle, 150, Easing.OutQuint);
            }
        }
    }

    protected override void PopIn() => cursor.FadeIn(200).ScaleTo(1, 400, Easing.OutQuint);
    protected override void PopOut() => cursor.FadeOut(200).ScaleTo(.8f, 400, Easing.OutQuint);

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        cursor.ShowOutline();
        mouseDownPosition = e.ScreenSpaceMousePosition;
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        cursor.HideOutline();

        mouseDownPosition = null;
        rotating = false;
        cursor.RotateTo(0, 400, Easing.OutQuint);
    }

    private partial class DrawableCursor : CompositeDrawable
    {
        private Container spriteContainer;
        private Sprite borderClick;
        private Sprite fillClick;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                spriteContainer = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            Texture = textures.Get("Cursor/fill.png")
                        },
                        fillClick = new Sprite
                        {
                            Texture = textures.Get("Cursor/fill-click.png"),
                            Alpha = 0
                        },
                        new Sprite
                        {
                            Texture = textures.Get("Cursor/border.png")
                        },
                        borderClick = new Sprite
                        {
                            Texture = textures.Get("Cursor/border-click.png"),
                            Alpha = 0
                        }
                    }
                }
            };
        }

        public void ShowOutline()
        {
            borderClick.FadeIn(200);
            fillClick.FadeIn(200);
            spriteContainer.ScaleTo(.8f, 1000, Easing.OutQuint);
        }

        public void HideOutline()
        {
            borderClick.FadeOut(200);
            fillClick.FadeOut(200);
            spriteContainer.ScaleTo(1f, 1000, Easing.OutElastic);
        }
    }
}
