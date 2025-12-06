using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollMultiplierTag : EditorTag
{
    public override Colour4 TagColour => Theme.ScrollMultiply;

    [Resolved]
    private EditorTagDependencies deps { get; set; }

    public ScrollMultiplierEvent ScrollMultiplier => (ScrollMultiplierEvent)TimedObject;

    public ScrollMultiplierTag(EditorTagContainer parent, ScrollMultiplierEvent sm)
        : base(parent, sm)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        ExpandedPadding = OriginalPadding with { Left = OriginalPadding.Left + 4, Right = OriginalPadding.Right + 4 };
    }

    protected override void Update()
    {
        base.Update();
        
        if (IsHovered)
            Text.Text = $"{ScrollMultiplier.Multiplier.ToStringInvariant("0.####")}x {Math.Floor(ScrollMultiplier.Duration)}ms | {ScrollMultiplier.Easing}";
        else
            Text.Text = $"{ScrollMultiplier.Multiplier.ToStringInvariant("0.####")}x {Math.Floor(ScrollMultiplier.Duration)}ms";
    }

    protected override bool OnClick(ClickEvent e)
    {
        deps.ShowPointFrom(deps.DesignPointsSidebar, ScrollMultiplier);
        return true;
    }
}