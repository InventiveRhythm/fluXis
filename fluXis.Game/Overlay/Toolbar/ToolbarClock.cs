using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarClock : Container
{
    private SpriteText clockText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 100;
        Padding = new MarginPadding { Horizontal = 10 };

        Add(clockText = new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Font = FluXisFont.Default(),
            Text = "00:00:00"
        });
    }

    protected override void Update()
    {
        clockText.Text = DateTime.Now.ToString("HH:mm:ss");
        base.Update();
    }
}
