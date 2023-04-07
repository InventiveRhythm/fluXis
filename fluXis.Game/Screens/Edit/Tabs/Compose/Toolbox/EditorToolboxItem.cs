using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Toolbox;

public partial class EditorToolboxItem : Container
{
    public IconUsage Icon
    {
        get => icon.Icon;
        set => icon.Icon = value;
    }

    public string Text
    {
        get => text.Text.ToString();
        set => text.Text = value;
    }

    public EditorTool Tool { get; init; }

    private readonly EditorPlayfield playfield;

    private readonly SpriteIcon icon;
    private readonly SpriteText text;
    private readonly Box hover;

    public EditorToolboxItem(EditorPlayfield playfield)
    {
        this.playfield = playfield;

        Height = 64;
        RelativeSizeAxes = Axes.X;
        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Surface2
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            icon = new SpriteIcon
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Size = new Vector2(32),
                Margin = new MarginPadding { Left = 64 }
            },
            text = new SpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 74 },
                Font = FluXisFont.Default(32)
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        playfield.Tool = Tool;
        hover.FadeTo(.4f).FadeTo(.2f, 200);

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f);
        return false; // dont "really" handle else the resize animation will not work
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }
}
