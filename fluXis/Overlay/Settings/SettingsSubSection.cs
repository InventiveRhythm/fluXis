using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings;

public partial class SettingsSubSection : FillFlowContainer
{
    [Resolved]
    protected FluXisConfig Config { get; private set; }

    public virtual LocalisableString Title => "Subsection";
    public virtual IconUsage Icon => FontAwesome6.Solid.AngleRight;

    public BindableBool Visible { get; } = new(true);

    protected FillFlowContainer RightSide { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(12);

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteIcon
                        {
                            Icon = Icon,
                            Size = new Vector2(24),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        },
                        new FluXisSpriteText
                        {
                            Text = Title,
                            WebFontSize = 28,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    }
                },
                RightSide = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(8)
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Visible.BindValueChanged(v => Alpha = v.NewValue ? 1f : 0f, true);
    }
}
