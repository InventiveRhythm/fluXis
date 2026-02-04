using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class ScrollMultiplierTag : EditorTag
{
    public override Colour4 TagColour => Theme.ScrollMultiply;

    private ScrollMultiplierEvent mult => (ScrollMultiplierEvent)TimedObject;

    private bool isHovered;
    private MarginPadding originalPadding;
    private MarginPadding expandedPadding;

    public ScrollMultiplierTag(EditorTagContainer parent, ScrollMultiplierEvent sm)
        : base(parent, sm)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        originalPadding = Padding;
        expandedPadding = originalPadding with { Left = originalPadding.Left + 4, Right = originalPadding.Right + 4 };
    }

    protected override void Update()
    {
        base.Update();

        if (isHovered)
            Text.Text = $"{mult.Multiplier.ToStringInvariant("0.####")}x {Math.Floor(mult.Duration)}ms | {mult.Easing}";
        else
            Text.Text = $"{mult.Multiplier.ToStringInvariant("0.####")}x {Math.Floor(mult.Duration)}ms";
    }

    protected override bool OnHover(HoverEvent e)
    {
        isHovered = true;
        this.TransformTo(nameof(Padding), expandedPadding, 200, Easing.OutQuart);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        isHovered = false;
        this.TransformTo(nameof(Padding), originalPadding, 200, Easing.OutQuart);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Editor.ChangeToTab<DesignTab>(x => x.Container.Sidebar.ShowPoint(mult));
        return true;
    }
}
