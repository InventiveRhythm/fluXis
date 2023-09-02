using System;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Overlay.Mouse;

public partial class Cursor : Container
{
    private readonly GlobalCursorOverlay overlay;

    private Container spriteContainer;
    private Sprite borderClick;
    private Sprite fillClick;
    private Container tooltipContainer;
    private Container tooltipContent;
    private bool visible;

    private InputManager inputManager;
    private Vector2? mouseDownPosition;
    private bool rotating;

    public Drawable DrawableTooltip
    {
        set
        {
            if (value == null)
            {
                if (visible)
                    tooltipContainer.FadeOut(200).ScaleTo(.9f, 400, Easing.OutQuint);

                visible = false;
                return;
            }

            tooltipContent.Clear();
            tooltipContent.Add(value);

            if (!visible)
                tooltipContainer.FadeIn(200).ScaleTo(1f, 600, Easing.OutElastic);

            var roundness = 0f;

            if (value is CompositeDrawable cornerRadius)
                roundness = cornerRadius.CornerRadius;

            tooltipContainer.CornerRadius = roundness == 0 ? 10 : roundness;
            visible = true;
        }
    }

    public Cursor(GlobalCursorOverlay overlay)
    {
        this.overlay = overlay;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
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
            },
            tooltipContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                X = 30,
                Y = 20,
                CornerRadius = 10,
                Scale = new Vector2(.9f),
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(.2f),
                    Radius = 5,
                    Offset = new Vector2(0, 1)
                },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    tooltipContent = new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

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
                var angle = MathUtils.RadiansToDegrees(MathF.Atan2(-mouseDelta.X, mouseDelta.Y)) + 24.3f;

                float delta = (angle - spriteContainer.Rotation) % 360;
                if (delta < -180) delta += 360;
                if (delta > 180) delta -= 360;
                angle = spriteContainer.Rotation + delta;

                spriteContainer.RotateTo(angle, 150, Easing.OutQuint);
            }
        }

        updateTooltipPosition();
    }

    private void updateTooltipPosition()
    {
        Quad tooltipScreenSpace = ToScreenSpace(tooltipContainer.DrawRectangle);

        if (tooltipScreenSpace.BottomRight.X > overlay.DrawWidth - 40)
            tooltipContainer.X = overlay.DrawWidth - tooltipScreenSpace.BottomRight.X - 10;
        else
            tooltipContainer.X = 30;

        if (tooltipScreenSpace.BottomRight.Y > overlay.DrawHeight - 40)
            tooltipContainer.Y = overlay.DrawHeight - tooltipScreenSpace.BottomRight.Y - 20;
        else
            tooltipContainer.Y = 20;
    }

    public override void Show()
    {
        borderClick.FadeIn(200);
        fillClick.FadeIn(200);
        spriteContainer.ScaleTo(.8f, 1000, Easing.OutQuint);
        mouseDownPosition = inputManager.CurrentState.Mouse.Position;
    }

    public override void Hide()
    {
        borderClick.FadeOut(200);
        fillClick.FadeOut(200);
        spriteContainer.ScaleTo(1f, 1000, Easing.OutElastic);

        mouseDownPosition = null;
        rotating = false;
        spriteContainer.RotateTo(0, 400, Easing.OutQuint);
    }
}
