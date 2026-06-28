using System;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;

public partial class TimelineTag : HoverClickContainer
{
    public virtual Colour4 TagColour => Colour4.White;
    public bool Expandable = true;

    protected FluXisSpriteText Text { get; private set; }
    protected virtual Action DeferredUpdateAction { get; set; }
    protected FluXisSpriteIcon Icon { get; private set; }
    private HoverClickContainer textContainer { get; set; }

    public ITimedObject TimedObject { get; }

    private EditorClock clock;

    private Vector2 collapsedSize = new(10, 10);
    private Vector2 expandedSize = new(14, 10 + text_margin);

    private const int string_limit = 30;
    private const float text_margin = 2;

    public Action OnDeferredUpdate { get; set; }
    public Action<float> AnimationEnd { get; set; }

    public new float X
    {
        get => base.X;
        set
        {
            // I have no idea why it's slightly offset by 2.5
            base.X = value + ((collapsedSize.X / 4) - 2.5f) / (Parent?.DrawWidth ?? 1f);

            if (IsLoaded)
            {
                DeferredUpdateAction?.Invoke();
                OnDeferredUpdate?.Invoke();
            }
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
                Icon = Phosphor.Bold.Play,
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
                HoverLostAction = () =>
                {
                    if (!IsHovered) Retract();
                },
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

        DeferredUpdateAction?.Invoke();
        OnDeferredUpdate?.Invoke();

        trimText();

        this.ResizeTo(expandedSize, 200, Easing.OutQuint);

        textContainer.Width = 16;
        textContainer.ResizeHeightTo(16, 100, Easing.In).Then().ResizeWidthTo(Text.DrawWidth + 6, 100, Easing.OutQuint);
        Text.FadeIn(150);
        AnimationEnd?.Invoke((float)(210 + Time.Current));
    }

    protected virtual void Retract()
    {
        if (!Expandable && !IsHovered)
            return;

        DeferredUpdateAction?.Invoke();
        OnDeferredUpdate?.Invoke();

        trimText();
        Text.FadeOut(100);
        textContainer.ResizeWidthTo(collapsedSize.X, 100, Easing.In).Then().ResizeHeightTo(0, 100, Easing.OutQuint);
        this.Delay(200).Then().ResizeTo(collapsedSize, 200, Easing.OutQuint);
        AnimationEnd?.Invoke((float)(410 + Time.Current));
    }

    protected override bool OnHover(HoverEvent e)
    {
        Expand();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        if (!textContainer.IsHovered)
            Retract();
    }
}
