using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarClock : Container
{
    private FluXisSpriteText localTime;
    private FluXisSpriteText gameTime;

    private double lastTime;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 100;
        Padding = new MarginPadding { Horizontal = 10 };

        InternalChildren = new Drawable[]
        {
            localTime = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.BottomCentre,
                Y = 5
            },
            gameTime = new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                FontSize = 14,
                Colour = FluXisColors.Text2
            }
        };
    }

    protected override void Update()
    {
        if (Time.Current - lastTime < 1000)
            return;

        lastTime = Time.Current;

        var time = Time.Current / 1000;
        gameTime.Text = $"{time / 360:00}:{time / 60:00}:{time % 60:00}";
        localTime.Text = DateTime.Now.ToString("HH:mm:ss");
    }
}
