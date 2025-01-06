using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Gameplay.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields.UI;

public partial class LaneSwitchAlert : Container
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    private LaneSwitchEvent currentEvent;

    private readonly SpriteIcon leftIcon;
    private readonly SpriteIcon rightIcon;

    public LaneSwitchAlert()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Children = new Drawable[]
        {
            new Container
            {
                X = -100,
                Size = new Vector2(100),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                Children = new Drawable[]
                {
                    leftIcon = new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Solid.AngleLeft,
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0
                    }
                }
            },
            new Container
            {
                X = 100,
                Size = new Vector2(100),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreLeft,
                Children = new Drawable[]
                {
                    rightIcon = new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Solid.AngleRight,
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
        if (playfield.Clock is not GameplayClock clock) return;

        if (!config.Get<bool>(FluXisSetting.LaneSwitchAlerts)) return;

        if (currentEvent == null)
            currentEvent = laneSwitchManager.Current;
        else
        {
            var time = clock.BeatTime;
            var fadeTime = time / 2;

            var nextEvent = ruleset.MapEvents.LaneSwitchEvents.Find(e => e.Time > clock.CurrentTime);

            if (nextEvent == null || nextEvent.Time - clock.CurrentTime > time)
                return;

            if (currentEvent.Count < nextEvent.Count)
            {
                leftIcon.X = 0;
                leftIcon.Rotation = 0;

                rightIcon.X = 0;
                rightIcon.Rotation = 0;

                leftIcon.FadeIn(fadeTime).Then().FadeOut(fadeTime);
                leftIcon.MoveToX(-100, time, Easing.OutQuint);

                rightIcon.FadeIn(fadeTime).Then().FadeOut(fadeTime);
                rightIcon.MoveToX(100, time, Easing.OutQuint);
            }
            else if (currentEvent.Count > nextEvent.Count)
            {
                leftIcon.X = -100;
                leftIcon.Rotation = 180;
                rightIcon.X = 100;
                rightIcon.Rotation = 180;

                leftIcon.FadeIn(fadeTime).Then().FadeOut(fadeTime);
                leftIcon.MoveToX(0, time, Easing.OutQuint);

                rightIcon.FadeIn(fadeTime).Then().FadeOut(fadeTime);
                rightIcon.MoveToX(0, time, Easing.OutQuint);
            }

            currentEvent = nextEvent;
        }
    }
}
