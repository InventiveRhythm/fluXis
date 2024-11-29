using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Setup;

public partial class SetupEntry : CompositeDrawable, IHasTooltip
{
    public LocalisableString TooltipText { get; set; }

    protected virtual float ContentSpacing => 4;
    protected virtual bool ShowHoverFlash => false;

    private HoverLayer hover;
    private FlashLayer flash;

    private string title { get; }

    public SetupEntry(string title)
    {
        this.title = title;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            hover = new HoverLayer(),
            flash = new FlashLayer(),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Padding = new MarginPadding(10),
                Spacing = new Vector2(ContentSpacing),
                Children = new[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new[]
                        {
                            new FluXisSpriteText
                            {
                                Text = title,
                                WebFontSize = 16,
                                Alpha = .8f
                            },
                            CreateRightTitle().With(d =>
                            {
                                d.Anchor = Anchor.TopRight;
                                d.Origin = Anchor.TopRight;
                            })
                        }
                    },
                    CreateContent()
                }
            }
        };
    }

    protected virtual Drawable CreateContent() => Empty();
    protected virtual Drawable CreateRightTitle() => Empty();

    protected override bool OnHover(HoverEvent e)
    {
        if (!ShowHoverFlash)
            return false;

        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e) => hover.Hide();

    protected override bool OnClick(ClickEvent e)
    {
        if (!ShowHoverFlash)
            return false;

        flash.Show();
        return true;
    }
}
