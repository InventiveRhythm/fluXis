using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using fluXis.Utils.Attributes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
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
                Colour = FluXisColors.Highlight,
                CornerRadius = 6,
                Masking = true,
                BorderThickness = 4,
                BorderColour = FluXisColors.Highlight.Lighten(1f),
                Child = new Box { RelativeSizeAxes = Axes.Both }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 36),
                    new()
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
                        }
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
}
