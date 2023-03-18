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
    public EditorToolbox(EditorPlayfield playfield)
    {
        AutoSizeAxes = Axes.Y;
        Width = 84;
        CornerRadius = 10;
        Masking = true;
        Alpha = 0.5f;

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
                Spacing = new Vector2(0, 10),
                Padding = new MarginPadding(10),
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
        this.FadeIn(200)
            .ResizeWidthTo(400, 200, Easing.OutQuint);

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.FadeTo(.5f, 200)
            .ResizeWidthTo(84, 200, Easing.OutQuint);
    }
}
