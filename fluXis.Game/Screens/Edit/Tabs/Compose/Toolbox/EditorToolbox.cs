using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Toolbox;

public partial class EditorToolbox : Container
{
    private const int padding = 5;
    private const int size_closed = 48 + padding * 2;
    private const int size_open = 200;

    public EditorToolbox(EditorPlayfield playfield)
    {
        RelativeSizeAxes = Axes.Y;
        Width = size_closed;
        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, padding),
                Padding = new MarginPadding(padding),
                Children = new Drawable[]
                {
                    new EditorToolboxItem(playfield)
                    {
                        Icon = FontAwesome.Solid.MousePointer,
                        Text = "Select",
                        Tool = EditorTool.Select
                    },
                    new EditorToolboxItem(playfield)
                    {
                        Icon = FontAwesome.Solid.Pen,
                        Text = "Single Note",
                        Tool = EditorTool.Single
                    },
                    new EditorToolboxItem(playfield)
                    {
                        Icon = FontAwesome.Solid.PencilRuler,
                        Text = "Long Note",
                        Tool = EditorTool.Long
                    },
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.ResizeWidthTo(size_open, 200, Easing.OutQuint);

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.ResizeWidthTo(size_closed, 200, Easing.OutQuint);
    }
}
