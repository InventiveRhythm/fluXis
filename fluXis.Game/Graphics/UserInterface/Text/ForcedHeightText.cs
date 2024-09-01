using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace fluXis.Game.Graphics.UserInterface.Text;

public partial class ForcedHeightText : CompositeDrawable
{
    public LocalisableString Text { get; init; }
    public float WebFontSize { set => FontSize = FluXisSpriteText.GetWebFontSize(value); }
    public float FontSize { get; set; } = 20;
    public bool Shadow { get; init; } = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;

        InternalChild = new FluXisSpriteText
        {
            Text = Text,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FontSize = FontSize,
            Shadow = Shadow
        };
    }
}
