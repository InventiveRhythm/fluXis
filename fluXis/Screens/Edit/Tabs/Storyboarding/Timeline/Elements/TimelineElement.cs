using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;

public partial class TimelineElement : CompositeDrawable
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    [Resolved]
    private ScriptStorage scripts { get; set; }

    public StoryboardElement Element { get; }

    private TruncatingText text;
    private DragHandle handle;

    public TimelineElement(StoryboardElement element)
    {
        Element = element;

        Name = $"{element.Type}";

        if (element.Type == StoryboardElementType.Sprite)
            Name += $" [{element.GetParameter("file", "")}]";
        if (element.Type == StoryboardElementType.Text)
            Name += $" [{element.GetParameter("text", "")}] ";
        if (element.Type == StoryboardElementType.Script)
            Name += $" [{element.GetParameter("path", "")}] ";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 36;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Highlight,
                CornerRadius = 6,
                Masking = true,
                BorderThickness = 4,
                BorderColour = Theme.Highlight.Lighten(1f),
                Child = new Box { RelativeSizeAxes = Axes.Both }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 36),
                    new(),
                    new(GridSizeMode.Absolute, 28)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new FluXisSpriteIcon
                        {
                            Size = new Vector2(16),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = Element.Type.GetIcon()
                        },
                        text = new TruncatingText
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Padding = new MarginPadding { Left = -2, Right = 14 },
                            WebFontSize = 12
                        },
                        handle = new DragHandle { Alpha = 0 }
                    }
                }
            },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        UpdateText();
    }

    public void UpdateText()
    {
        switch (Element.Type)
        {
            case StoryboardElementType.Text:
                text.Text = Element.GetParameter("text", "");
                break;

            case StoryboardElementType.Sprite:
                text.Text = Element.GetParameter("file", "");
                break;

            case StoryboardElementType.Script:
            {
                var path = Element.GetParameter("path", "");
                var script = scripts.Scripts.FirstOrDefault(x => x.Path.EqualsLower(path));
                var txt = path;

                if (script is not null)
                {
                    var args = script.Parameters.Select(parameter => Element.GetParameter<object>(parameter.Key, "")).ToList();
                    txt += $" ({string.Join(", ", args)})";
                }

                text.Text = txt;
                break;
            }

            default:
                text.Text = "";
                break;
        }
    }

    protected override void Update()
    {
        base.Update();

        var start = timeline.PositionAtTime(Element.StartTime);
        var end = timeline.PositionAtTime(Element.EndTime);

        Width = end - start;
        X = start;
        Y = timeline.PositionAtZ(Element.ZIndex);
    }

    protected override bool OnHover(HoverEvent e)
    {
        handle.FadeIn(200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        handle.FadeOut(200);
    }

    /// <summary>
    /// for the actual dragging see <see cref="TimelineElementBlueprint.BlueprintHandle"/>
    /// </summary>
    private partial class DragHandle : CompositeDrawable
    {
        private Circle handle { get; }

        public DragHandle()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                handle = new Circle
                {
                    Size = new Vector2(4, 16),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .6f
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            handle.FadeTo(1f, 100).ResizeWidthTo(8, 200, Easing.OutQuint);
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            handle.FadeTo(.6f, 600).ResizeWidthTo(4, 400, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }
}
