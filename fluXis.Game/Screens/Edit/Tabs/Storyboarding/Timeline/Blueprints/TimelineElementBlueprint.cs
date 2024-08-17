using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;

public partial class TimelineElementBlueprint : SelectionBlueprint<StoryboardElement>
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    public override double FirstComparer => Object.StartTime;
    public override double SecondComparer => Object.EndTime;

    public override Vector2 ScreenSpaceSelectionPoint => Drawable.ScreenSpaceDrawQuad.TopLeft;

    public TimelineElementBlueprint(StoryboardElement element)
        : base(element)
    {
        Height = 36;
        Anchor = Origin = Anchor.TopLeft;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 6,
            Masking = true,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .2f
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Parent == null)
            return;

        var start = Parent.ToLocalSpace(timeline.ScreenSpacePositionAtTime(Object.StartTime, Object.ZIndex));
        var end = Parent.ToLocalSpace(timeline.ScreenSpacePositionAtTime(Object.EndTime, Object.ZIndex));

        Position = start;
        Width = end.X - start.X;
    }
}
