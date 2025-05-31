using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.BottomBar;

public partial class EditorPlayTestButton : ClickableContainer
{
    [Resolved]
    private UISamples samples { get; set; }

    private HoverLayer hover;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            hover = new HoverLayer(),
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
        hover.Show();
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
        hover.Hide();
        base.OnHoverLost(e);
    }
}
