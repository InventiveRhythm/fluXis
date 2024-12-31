using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;

public partial class TimelineElement : CompositeDrawable
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    public StoryboardElement Element { get; }

    public TimelineElement(StoryboardElement element)
    {
        Element = element;

        Name = $"{element.Type}";

        if (element.Type == StoryboardElementType.Sprite)
            Name += $" [{element.Parameters["file"]}]";
        if (element.Type == StoryboardElementType.Text)
            Name += $" [{element.Parameters["text"]}] ";
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
            new TruncatingText
            {
                RelativeSizeAxes = Axes.X,
                Text = createText(),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Padding = new MarginPadding { Horizontal = 14 },
                WebFontSize = 12,
                Alpha = .75f
            }
        };
    }

    private string createText()
    {
        switch (Element.Type)
        {
            case StoryboardElementType.Text:
                return Element.Parameters["text"]?.ToString();

            case StoryboardElementType.Sprite:
                return Element.Parameters["file"]?.ToString();
        }

        return "";
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
