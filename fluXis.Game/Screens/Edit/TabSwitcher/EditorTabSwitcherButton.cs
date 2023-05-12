using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.TabSwitcher;

public partial class EditorTabSwitcherButton : ClickableContainer
{
    public string Text { get; set; }

    private Box background;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
        CornerRadius = 5;
        Masking = true;
        Children = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White,
                Alpha = 0
            },
            new SpriteText
            {
                Text = Text,
                Font = FluXisFont.Default(22),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding { Horizontal = 15 }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeTo(0, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        background.FadeTo(.4f).FadeTo(.2f, 200);
        return base.OnClick(e);
    }
}
