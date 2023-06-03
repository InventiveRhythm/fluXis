using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarClock : Container
{
    private FluXisSpriteText clockText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 100;
        Padding = new MarginPadding { Horizontal = 10 };

        Add(clockText = new FluXisSpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = "00:00:00"
        });
    }

    protected override void Update()
    {
        clockText.Text = DateTime.Now.ToString("HH:mm:ss");
        base.Update();
    }
}
