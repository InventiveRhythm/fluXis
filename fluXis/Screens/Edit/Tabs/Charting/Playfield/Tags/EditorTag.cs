using fluXis.Configuration;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTag : Container
{
    public virtual Colour4 TagColour => Colour4.White;

    [Resolved]
    private EditorPlayfield playfield { get; set; }

    private EditorTagContainer parent { get; }

    private Bindable<ScrollDirection> scrollDirection;

    public ITimedObject TimedObject { get; }
    protected FluXisSpriteText Text { get; private set; }
    public bool RightSide { get; set; }

    protected new bool IsHovered;
    protected MarginPadding OriginalPadding;
    protected MarginPadding ExpandedPadding;

    protected Container Container { get; private set; }

    public EditorTag(EditorTagContainer parent, ITimedObject timedObject)
    {
        this.parent = parent;
        TimedObject = timedObject;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AutoSizeAxes = Axes.X;
        Height = 20;
        Anchor = RightSide ? Anchor.TopLeft : Anchor.TopRight;
        Origin = RightSide ? Anchor.CentreLeft : Anchor.CentreRight;
        Padding = new MarginPadding
        {
            Left = RightSide ? 6 : 0,
            Right = !RightSide ? 6 : 0
        };

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(14),
                Anchor = RightSide ? Anchor.CentreLeft : Anchor.CentreRight,
                Origin = RightSide ? Anchor.BottomLeft : Anchor.TopRight,
                X = RightSide ? -7f : 7f,
                Rotation = 45,
                CornerRadius = 2,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = TagColour
                    }
                }
            },
            Container = new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                Origin = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                CornerRadius = 6,
                Masking = true,
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
                        Margin = new MarginPadding { Horizontal = 12 },
                        Colour = Theme.IsBright(TagColour) ? Colour4.Black : Colour4.White,
                        Alpha = .75f,
                        WebFontSize = 12
                    }
                }
            }
        };

        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);
    }

    private void updateScale() => Scale = new Vector2(1, scrollDirection.Value == ScrollDirection.Up ? -1 : 1);

    protected override void LoadComplete()
    {
        base.LoadComplete();

        OriginalPadding = Padding;
        ExpandedPadding = OriginalPadding with { Left = OriginalPadding.Left + 6, Right = OriginalPadding.Right + 6 };
        scrollDirection.BindValueChanged(_ => updateScale(), true);
    }

    protected override void Update()
    {
        base.Update();

        Y = parent.ToLocalSpace(playfield.HitObjectContainer.ScreenSpacePositionAtTime(TimedObject.Time, 0)).Y;
    }

    protected override bool OnHover(HoverEvent e)
    {
        IsHovered = true;
        this.TransformTo(nameof(Padding), ExpandedPadding, 200, Easing.OutQuart);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        IsHovered = false;
        this.TransformTo(nameof(Padding), OriginalPadding, 200, Easing.OutQuart);
        base.OnHoverLost(e);
    }
}
