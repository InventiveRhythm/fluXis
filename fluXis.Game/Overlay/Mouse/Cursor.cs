using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Overlay.Mouse;

public partial class Cursor : Container
{
    private readonly GlobalCursorOverlay overlay;

    private Sprite clickSprite;
    private FluXisSpriteText tooltipText;
    private Container tooltipContainer;

    public string TooltipText
    {
        set
        {
            if (value == "" && tooltipText.Text != "")
            {
                tooltipContainer.FadeOut(200);
            }
            else
            {
                tooltipText.Text = value;
                tooltipContainer.FadeIn(200);
            }
        }
    }

    public Cursor(GlobalCursorOverlay overlay)
    {
        this.overlay = overlay;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Add(new Sprite
        {
            Texture = textures.Get("Cursor/cursor.png")
        });

        Add(clickSprite = new Sprite
        {
            Texture = textures.Get("Cursor/cursorclick.png"),
            Alpha = 0
        });

        Add(tooltipContainer = new Container
        {
            AutoSizeAxes = Axes.Both,
            X = 30,
            Y = 20,
            CornerRadius = 5,
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
                    Colour = FluXisColors.Surface
                },
                tooltipText = new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Margin = new MarginPadding { Horizontal = 5, Vertical = 2 },
                    Shadow = true
                }
            }
        });
    }

    protected override void Update()
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
        clickSprite.FadeIn(200);
    }

    public override void Hide()
    {
        clickSprite.FadeOut(200);
    }
}
