using fluXis.Screens.Edit.Tabs.Shared.Lines;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Lines;

public partial class StoryboardTimingLines : EditorTimingLines<StoryboardTimingLines.StoryboardLine>
{
    public Anchor LineAnchor { get; init; } = Anchor.BottomLeft;
    public Anchor LineOrigin { get; init; } = Anchor.BottomCentre;

    private StoryboardTimeline timeline { get; }

    public StoryboardTimingLines(StoryboardTimeline timeline)
    {
        this.timeline = timeline;
    }

    protected override StoryboardLine CreateLine(double time, bool big, Colour4 color) => new(this, time)
    {
        Height = big ? 1f : .6f,
        Colour = color
    };

    protected override Vector2 GetPosition(double time) => new(timeline.PositionAtTime(time, DrawWidth), 0);

    public partial class StoryboardLine : Line
    {
        public override bool BelowScreen => parent.GetPosition(Time).X < 0;
        public override bool AboveScreen => parent.GetPosition(Time).X > Parent.DrawWidth;

        private StoryboardTimingLines parent { get; }

        public StoryboardLine(StoryboardTimingLines parent, double time)
            : base(parent, time)
        {
            this.parent = parent;

            Width = 2;
            RelativeSizeAxes = Axes.Y;

            Anchor = parent.LineAnchor;
            Origin = parent.LineOrigin;
        }
    }
}
