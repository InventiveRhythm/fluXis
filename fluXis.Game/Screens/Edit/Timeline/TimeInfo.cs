using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Timeline;

public partial class TimeInfo : Container
{
    public EditorBottomBar BottomBar { get; set; }

    private SpriteText timeText;
    private SpriteText bpmText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(10);

        Children = new Drawable[]
        {
            timeText = new SpriteText
            {
                Font = FluXisFont.Default(28, true),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding { Bottom = -5 }
            },
            bpmText = new SpriteText
            {
                Font = FluXisFont.Default(),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.TopLeft,
                Colour = FluXisColors.Text2
            }
        };
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Format(Conductor.CurrentTime);
        bpmText.Text = $"{BottomBar.Editor.MapInfo.GetTimingPoint(Conductor.CurrentTime)?.BPM} BPM";
    }
}
