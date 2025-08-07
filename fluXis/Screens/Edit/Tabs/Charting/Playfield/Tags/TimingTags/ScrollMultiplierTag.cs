using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollMultiplierTag : EditorTag
{
    public override Colour4 TagColour => Theme.ScrollMultiply;

    [Resolved]
    private PointsSidebar points { get; set; }

    public ScrollMultiplierEvent ScrollMultiplier => (ScrollMultiplierEvent)TimedObject;

    public ScrollMultiplierTag(EditorTagContainer parent, ScrollMultiplierEvent sm)
        : base(parent, sm)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{ScrollMultiplier.Multiplier.ToStringInvariant("0.####")}x {Math.Floor(ScrollMultiplier.Duration)}ms";
    }

    protected override bool OnClick(ClickEvent e)
    {
        points.ShowPoint(ScrollMultiplier);
        return true;
    }
}
