using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Tests.Graphics;

public partial class TestMarquee : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var marquee = new Marquee
        {
            Width = 500,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
        Add(marquee);

        AddSliderStep("marquee width", 0, 1200, Width, v => marquee.Width = v);

        AddStep("set to long text", () => marquee.CreateFunc = () => new FluXisSpriteText
        {
            Text = "this is a very very VERY long text and it should scroll inside the marquee container because its long, you get what i mean?",
            WebFontSize = 20
        });

        AddStep("set to medium text", () => marquee.CreateFunc = () => new FluXisSpriteText
        {
            Text = "medium length text that can be used to text with resizing",
            WebFontSize = 20
        });

        AddStep("set to short text", () => marquee.CreateFunc = () => new FluXisSpriteText
        {
            Text = "not scrolling text",
            WebFontSize = 20
        });
    }
}
