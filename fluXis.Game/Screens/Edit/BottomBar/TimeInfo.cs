using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game.Screens.Edit.BottomBar;

public partial class TimeInfo : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Box hover;
    private Box flash;
    private FluXisSpriteText timeText;
    private FluXisSpriteText bpmText;

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
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(15),
                Spacing = new Vector2(-3),
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    timeText = new FluXisSpriteText
                    {
                        FixedWidth = true,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 24
                    },
                    bpmText = new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 18,
                        Alpha = .8f
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Format(clock.CurrentTime);
        bpmText.Text = $"{map.MapInfo.GetTimingPoint((float)clock.CurrentTime)?.BPM} BPM";
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        clipboard.SetText(((int)clock.CurrentTime).ToString());
        notifications.SendSmallText("Copied current time to clipboard.");

        samples.Click();
        flash.FadeOutFromOne(1000, Easing.OutQuint);

        return true;
    }
}
