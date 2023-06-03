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
    private EditorTool? currentTool;

    private readonly SpriteIcon icon;
    private readonly FluXisSpriteText text;
    private readonly Box hover;

    public EditorToolboxItem(EditorPlayfield playfield)
    {
        this.playfield = playfield;

        Height = 48;
        RelativeSizeAxes = Axes.X;
        CornerRadius = 5;
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
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12, 0),
                Padding = new MarginPadding(12),
                Children = new Drawable[]
                {
                    icon = new SpriteIcon
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(24)
                    },
                    text = new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (currentTool != playfield.Tool)
        {
            currentTool = playfield.Tool;
            hover.FadeTo(currentTool == Tool ? .1f : 0, 200);
        }
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
        hover.FadeTo(playfield.Tool == Tool ? .1f : 0, 200);
    }
}
