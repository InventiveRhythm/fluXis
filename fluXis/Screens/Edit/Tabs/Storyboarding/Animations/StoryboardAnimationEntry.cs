using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations;

public partial class StoryboardAnimationEntry : CompositeDrawable
{
    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    private readonly StoryboardAnimation animation;
    private readonly StoryboardAnimationRow row;

    public StoryboardAnimationEntry(StoryboardAnimation animation, StoryboardAnimationRow row)
    {
        this.animation = animation;
        this.row = row;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        Size = new Vector2(StoryboardAnimationsList.ROW_HEIGHT);
        InternalChild = new FluXisSpriteIcon
        {
            Icon = FontAwesome6.Solid.Diamond,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.6f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    protected override void Update()
    {
        base.Update();
        X = timeline.PositionAtTime(animation.StartTime, Parent!.DrawWidth);
    }
}
