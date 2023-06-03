using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class EditorPlayTestButton : ClickableContainer
{
    private Box background;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new FluXisSpriteText
            {
                Text = "Test!",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeTo(.2f, 200);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        background.FadeTo(.4f, 100);
        return false;
    }

    protected override bool OnClick(ClickEvent e)
    {
        background.FadeTo(.2f, 200);
        return base.OnClick(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeOut(200);
        base.OnHoverLost(e);
    }
}
