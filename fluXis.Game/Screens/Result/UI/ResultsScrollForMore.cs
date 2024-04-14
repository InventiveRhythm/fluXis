using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultsScrollForMore : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 100; // maybe figure out a way to make this dynamic

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(10),
            Children = new Drawable[]
            {
                new SpriteIcon
                {
                    Icon = FontAwesome.Solid.AngleDoubleDown,
                    Size = new Vector2(25),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = "Scroll down for more",
                    WebFontSize = 20,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Shadow = true
                },
                new SpriteIcon
                {
                    Icon = FontAwesome.Solid.AngleDoubleDown,
                    Size = new Vector2(25),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Shadow = true
                }
            }
        };
    }
}
