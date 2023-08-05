using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Scroll;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Toolbox;

public partial class EditorToolbox : ExpandingContainer
{
    private const int padding = 5;
    private const int size_closed = 48 + padding * 2;
    private const int size_open = 200 + padding * 2;

    protected override double HoverDelay => 500;

    public EditorPlayfield Playfield { get; init; }

    private SectionTitle toolsTitle;

    private float titleX = -size_open;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = size_closed;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, padding),
                    Padding = new MarginPadding(padding),
                    Children = new Drawable[]
                    {
                        toolsTitle = new SectionTitle
                        {
                            TextExpanded = "Tools (0-9)",
                            TextCollapsed = "0-9"
                        },
                        new EditorToolboxItem(Playfield)
                        {
                            Icon = FontAwesome.Solid.MousePointer,
                            Text = "Select",
                            Tool = EditorTool.Select
                        },
                        new EditorToolboxItem(Playfield)
                        {
                            Icon = FontAwesome.Solid.Pen,
                            Text = "Single Note",
                            Tool = EditorTool.Single
                        },
                        new EditorToolboxItem(Playfield)
                        {
                            Icon = FontAwesome.Solid.PencilRuler,
                            Text = "Long Note",
                            Tool = EditorTool.Long
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuint);
            updateTitles(v.NewValue);
        }, true);
    }

    protected override void Update()
    {
        base.Update();

        toolsTitle.Margin = new MarginPadding { Left = titleX };
    }

    private void updateTitles(bool expanded)
    {
        var x = expanded ? 0f : -(float)size_open;
        this.TransformTo(nameof(titleX), x, 300, Easing.OutQuint);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;

        switch (e.Key)
        {
            case Key.Number1:
                Playfield.Tool = EditorTool.Select;
                return true;

            case Key.Number2:
                Playfield.Tool = EditorTool.Single;
                return true;

            case Key.Number3:
                Playfield.Tool = EditorTool.Long;
                return true;
        }

        return false;
    }

    private partial class SectionTitle : Container
    {
        public string TextExpanded { get; set; } = "";
        public string TextCollapsed { get; set; } = "";

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = size_open;
            Height = 20;
            X = -size_open;

            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = TextExpanded
                },
                new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    X = size_open,
                    Text = TextCollapsed
                }
            };
        }
    }
}
