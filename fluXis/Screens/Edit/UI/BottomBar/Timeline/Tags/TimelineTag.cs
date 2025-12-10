using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;

public partial class TimelineTag : Container
{
    public virtual Colour4 TagColour => Colour4.White;
    public bool Expandable = true;

    protected FluXisSpriteText Text { get; private set; }
    protected virtual Action UpdateAction { get; set; }
    protected FluXisSpriteIcon Icon { get; private set; }
    private HoverClickContainer textContainer { get; set; }

    public ITimedObject TimedObject { get; }

    private EditorClock clock;

    private Vector2 collapsedSize = new(10, 10);
    private Vector2 expandedSize = new(14, 10 + text_margin);

    private const int string_limit = 30;
    private const float text_margin = 2;

    public new float X
    {
        get => base.X;
        set
        {
            base.X = value + (collapsedSize.X / 4) / (Parent?.DrawWidth ?? 1f);
            UpdateAction?.Invoke();
        }
    }

    public TimelineTag(EditorClock clock, ITimedObject timedObject)
    {
        TimedObject = timedObject;
        this.clock = clock;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativePositionAxes = Axes.X;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        Size = collapsedSize;

        InternalChildren = new Drawable[]
        {
            Icon = new FluXisSpriteIcon
            {
                Icon = FontAwesome6.Solid.Play,
                Size = new Vector2(10),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 90,
                Colour = TagColour
            },
            textContainer = new HoverClickContainer
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.BottomCentre,
                Y = 0,
                Size = new Vector2(0, 0),
                Masking = true,
                HoverLostAction = () => { if (!IsHovered) Retract(); },
                Action = () => clock.SeekSmoothly(TimedObject.Time),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = TagColour
                    },
                    Text = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Theme.IsBright(TagColour) ? Theme.TextDark : Theme.Text,
                        FontSize = 16,
                        Alpha = 0
                    }
                }
            }
        };
    }

    private void trimText()
    {
        string textString = Text.Text.ToString();
        Text.Text = textString.Length > string_limit
            ? textString[..(string_limit - 3)] + "..."
            : textString;
    }

    protected virtual void Expand()
    {
        if (!Expandable)
            return;

        UpdateAction?.Invoke();
        
        trimText();

        this.ResizeTo(expandedSize, 200, Easing.OutQuint);

        textContainer.Width = 16;
        textContainer.ResizeHeightTo(16, 100, Easing.In).Then().ResizeWidthTo(Text.DrawWidth + 6, 100, Easing.OutQuint);
        Text.FadeIn(150);
    }

    protected virtual void Retract()
    {   
        if (!Expandable && !IsHovered)
            return;

        UpdateAction?.Invoke();

        trimText();
        Text.FadeOut(100);
        textContainer.ResizeWidthTo(collapsedSize.X, 100, Easing.In).Then().ResizeHeightTo(0, 100, Easing.OutQuint);
        this.Delay(200).Then().ResizeTo(collapsedSize, 200, Easing.OutQuint);
    }

    protected override bool OnHover(HoverEvent e)
    {
        Expand();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        if (!textContainer.IsHovered)
            Retract();
    }
}
