using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class TimeInfo : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    private FluXisSpriteText timeText;
    private FluXisSpriteText bpmText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(10);

        Children = new Drawable[]
        {
            timeText = new FluXisSpriteText
            {
                FontSize = 28,
                FixedWidth = true,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Bottom = -5 }
            },
            bpmText = new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.TopLeft,
                Colour = FluXisColors.Text2
            }
        };
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Format(clock.CurrentTime);
        bpmText.Text = $"{values.Editor.MapInfo.GetTimingPoint((float)clock.CurrentTime)?.BPM} BPM";
    }
}
