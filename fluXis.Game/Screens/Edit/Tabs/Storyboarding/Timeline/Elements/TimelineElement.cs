using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;

public partial class TimelineElement : CompositeDrawable
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    private StoryboardElement element { get; }

    public TimelineElement(StoryboardElement element)
    {
        this.element = element;

        Name = $"{element.Type}";

        if (element.Type == StoryboardElementType.Sprite)
            Name += $" [{element.Parameters["file"]}]";
        if (element.Type == StoryboardElementType.Text)
            Name += $" [{element.Parameters["text"]}] ";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 32;

        InternalChildren = new Drawable[]
        {
            new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Highlight,
                BorderThickness = 4,
                BorderColour = FluXisColors.Highlight.Lighten(1f)
            },
            new TruncatingText
            {
                RelativeSizeAxes = Axes.X,
                Text = createText(),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Padding = new MarginPadding { Horizontal = 16 },
                WebFontSize = 12,
                Alpha = .75f
            }
        };
    }

    private string createText()
    {
        switch (element.Type)
        {
            case StoryboardElementType.Text:
                return element.Parameters["text"]?.ToString();

            case StoryboardElementType.Sprite:
                return element.Parameters["file"]?.ToString();
        }

        return "";
    }

    protected override void Update()
    {
        base.Update();

        var start = timeline.PositionAtTime(element.StartTime);
        var end = timeline.PositionAtTime(element.EndTime);

        Width = end - start;
        X = start;
        Y = timeline.PositionAtZ(element.ZIndex);
    }
}
