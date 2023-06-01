using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class LaneSwitchAlert : Container
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    public Playfield Playfield { get; set; }

    private LaneSwitchEvent currentEvent;

    private readonly Container leftContainer;
    private readonly Container rightContainer;
    private readonly SpriteIcon leftIcon;
    private readonly SpriteIcon rightIcon;

    public LaneSwitchAlert()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Children = new Drawable[]
        {
            leftContainer = new Container
            {
                Size = new Vector2(100),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    leftIcon = new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.ChevronLeft,
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0
                    }
                }
            },
            rightContainer = new Container
            {
                Size = new Vector2(100),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    rightIcon = new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.ChevronRight,
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        leftContainer.X = -Playfield.Stage.Width / 2 - 100;
        rightContainer.X = Playfield.Stage.Width / 2 + 100;

        if (!config.Get<bool>(FluXisSetting.LaneSwitchAlerts))
            return;

        if (currentEvent == null)
            currentEvent = Playfield.Manager.CurrentLaneSwitchEvent;
        else
        {
            var nextEvent = Playfield.Screen.MapEvents.LaneSwitchEvents.Find(e => e.Time > clock.CurrentTime);

            if (nextEvent == null || nextEvent.Time - clock.CurrentTime > clock.BeatTime)
                return;

            if (currentEvent.Count < nextEvent.Count)
            {
                leftIcon.X = 0;
                leftIcon.Rotation = 0;

                rightIcon.X = 0;
                rightIcon.Rotation = 0;

                leftIcon.FadeIn(clock.BeatTime / 2).Then().FadeOut(clock.BeatTime / 2);
                leftIcon.MoveToX(-100, clock.BeatTime, Easing.OutQuint);

                rightIcon.FadeIn(clock.BeatTime / 2).Then().FadeOut(clock.BeatTime / 2);
                rightIcon.MoveToX(100, clock.BeatTime, Easing.OutQuint);
            }
            else if (currentEvent.Count > nextEvent.Count)
            {
                leftIcon.X = -100;
                leftIcon.Rotation = 180;
                rightIcon.X = 100;
                rightIcon.Rotation = 180;

                leftIcon.FadeIn(clock.BeatTime / 2).Then().FadeOut(clock.BeatTime / 2);
                leftIcon.MoveToX(0, clock.BeatTime, Easing.OutQuint);

                rightIcon.FadeIn(clock.BeatTime / 2).Then().FadeOut(clock.BeatTime / 2);
                rightIcon.MoveToX(0, clock.BeatTime, Easing.OutQuint);
            }

            currentEvent = nextEvent;
        }

        base.Update();
    }
}
