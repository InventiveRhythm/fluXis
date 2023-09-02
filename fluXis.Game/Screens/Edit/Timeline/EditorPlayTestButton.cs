using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class EditorPlayTestButton : ClickableContainer
{
    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            hover = new Box
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
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        return base.OnClick(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
        base.OnHoverLost(e);
    }
}
