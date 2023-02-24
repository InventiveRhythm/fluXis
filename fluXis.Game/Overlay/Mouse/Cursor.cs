using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Overlay.Mouse;

public partial class Cursor : Container
{
    private Sprite clickSprite;
    private SpriteText tooltipText;
    private Container tooltipContainer;

    public string TooltipText
    {
        set
        {
            if (value == "")
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

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Add(new Sprite
        {
            Texture = textures.Get("Cursor/cursor.png"),
        });

        Add(clickSprite = new Sprite
        {
            Texture = textures.Get("Cursor/cursorclick.png"),
            Alpha = 0,
        });

        Add(tooltipContainer = new Container
        {
            AutoSizeAxes = Axes.Both,
            X = 50,
            Y = 70,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Surface,
                },
                tooltipText = new SpriteText
                {
                    Font = new FontUsage("QuickSand", 20, "Bold"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Margin = new MarginPadding { Horizontal = 10 },
                    Y = -2,
                },
            }
        });
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
