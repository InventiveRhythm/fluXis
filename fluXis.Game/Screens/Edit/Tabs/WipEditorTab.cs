using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class WipEditorTab : EditorTab
{
    public override IconUsage Icon { get; }
    public override string TabName { get; }
    private string description { get; }

    private Container content;

    public WipEditorTab(IconUsage icon, string tabName, string description)
    {
        Icon = icon;
        TabName = tabName;
        this.description = description;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new Container
        {
            AutoSizeAxes = Axes.Both,
            CornerRadius = 20,
            Masking = true,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2,
                    Alpha = .6f
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(20) { Top = 30 },
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Icon = Icon,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Size = new Vector2(64),
                            Margin = new MarginPadding { Bottom = 20 }
                        },
                        new FluXisSpriteText
                        {
                            Text = TabName,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 24
                        },
                        new FluXisSpriteText
                        {
                            Text = "This tab is still work in progress.",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 16
                        },
                        new FluXisSpriteText
                        {
                            Text = description,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            WebFontSize = 16,
                            Alpha = .8f
                        }
                    }
                }
            }
        };
    }

    public override void Show()
    {
        base.Show();

        content.ScaleTo(.75f)
               .FadeInFromZero(400, Easing.OutQuint)
               .ScaleTo(1f, 800, Easing.OutElasticHalf);
    }
}
