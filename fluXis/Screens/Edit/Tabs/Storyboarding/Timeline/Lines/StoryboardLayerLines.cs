using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Lines;

public partial class StoryboardLayerLines : Container
{
    [Resolved]
    private EditorMap map { get; set; }

    private int highestZ;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        updateHighestZ(10);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        map.Storyboard.ElementAdded += e => updateHighestZ(e.ZIndex);
        map.Storyboard.ElementUpdated += e => updateHighestZ(e.ZIndex);
    }

    private void updateHighestZ(int z)
    {
        z = Math.Max(z, highestZ);

        var current = highestZ;
        var extra = z - highestZ;

        if (extra is 0) return;

        for (int i = 0; i < extra; i++)
        {
            AddInternal(new Line(current));
            current++;
        }

        highestZ = z;
    }

    private partial class Line : Box
    {
        [Resolved]
        private StoryboardTimeline timeline { get; set; }

        private readonly int z;

        public Line(int z)
        {
            this.z = z;
            RelativeSizeAxes = Axes.X;
            Height = 1;
            Colour = Theme.Background3;
        }

        protected override void Update()
        {
            base.Update();
            Y = timeline.PositionAtZ(z) + TimelineElement.HEIGHT + (int)(StoryboardTimeline.ELEMENT_SPACING / 2f);
        }
    }
}
