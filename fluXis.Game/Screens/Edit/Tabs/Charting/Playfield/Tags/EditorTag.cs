using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTag : Container
{
    public virtual Colour4 TagColour => Colour4.White;
    public virtual int TagWidth => 80;

    [Resolved]
    private EditorPlayfield playfield { get; set; }

    private EditorTagContainer parent { get; }

    public ITimedObject TimedObject { get; }
    protected FluXisSpriteText Text { get; private set; }
    public bool RightSide { get; set; }

    public EditorTag(EditorTagContainer parent, ITimedObject timedObject)
    {
        this.parent = parent;
        TimedObject = timedObject;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = TagWidth + 6;
        Height = 20;
        Anchor = Anchor.TopRight;
        Origin = Anchor.CentreRight;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Width = TagWidth,
                RelativeSizeAxes = Axes.Y,
                Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                Origin = RightSide ? Anchor.CentreRight : Anchor.CentreLeft,
                CornerRadius = 5,
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
                        Colour = FluXisColors.IsBright(TagColour) ? Colour4.Black : Colour4.White,
                        Alpha = .75f,
                        FontSize = 14
                    }
                }
            },
            new Container
            {
                Size = new Vector2(12),
                Anchor = RightSide ? Anchor.CentreLeft : Anchor.CentreRight,
                Origin = RightSide ? Anchor.BottomLeft : Anchor.TopRight,
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
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        Y = parent.ToLocalSpace(playfield.HitObjectContainer.ScreenSpacePositionAtTime(TimedObject.Time, 0)).Y;
    }
}
