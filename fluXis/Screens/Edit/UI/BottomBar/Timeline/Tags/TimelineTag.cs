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
    protected FluXisSpriteIcon Icon { get; private set; }
    protected Container TextContainer { get; private set; }
    protected Box Background { get; private set; }

    public ITimedObject TimedObject { get; }

    private EditorClock clock;

    private Vector2 collapsedSize = new Vector2(10, 10);
    private Vector2 expandedSize = new Vector2(30, 10);

    public new float X
    {
        get => base.X;
        set => base.X = value + (collapsedSize.X / 4) / (Parent?.DrawWidth ?? 1f);
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
                X = -collapsedSize.X / 4,
                Colour = TagColour
            },
            TextContainer = new Container
            {
                X = -collapsedSize.X / 4,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.BottomCentre,
                Y = 0,
                Size = new Vector2(0, 0),
                Masking = true,
                Children = new Drawable[]
                {
                    Background = new Box
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
        Text.Text = textString.Length > 25 
            ? textString[..22] + "..." 
            : textString;
    }

    public void Expand()
    {
        if (!Expandable)
            return;
        
        trimText();

        this.ResizeTo(expandedSize, 200, Easing.OutQuint);

        TextContainer.Width = 16;
        TextContainer.ResizeHeightTo(16, 100, Easing.In).Then().ResizeWidthTo(Text.DrawWidth + 6, 100, Easing.OutQuint);
        Text.FadeIn(150);
        
        OnExpand();
    }

    public void Retract()
    {   
        trimText();
        Text.FadeOut(100);
        TextContainer.ResizeWidthTo(10, 100, Easing.In).Then().ResizeHeightTo(0, 100, Easing.OutQuint);
        this.Delay(200).Then().ResizeTo(collapsedSize, 200, Easing.OutQuint);
        OnRetract();
    }

    protected override bool OnClick(ClickEvent e)
    {
        clock.SeekSmoothly(TimedObject.Time);
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        Expand();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        Retract();
    }

    protected virtual void OnExpand() { }
    protected virtual void OnRetract() { }
}
