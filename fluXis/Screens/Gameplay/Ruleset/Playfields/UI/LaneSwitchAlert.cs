using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures.Events;
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

    private IList<LaneSwitchEvent> events => ruleset.MapEvents.LaneSwitchEvents;

    private SpriteIcon leftIcon;
    private SpriteIcon rightIcon;

    [BackgroundDependencyLoader]
    private void load()
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

        build();
    }

    private void build()
    {
        ClearTransforms();

        if (events.Count == 0 || !config.Get<bool>(FluXisSetting.LaneSwitchAlerts))
            return;

        var last = events.First();

        for (int i = 1; i < events.Count; i++)
            var current = events[i];

            if (nextEvent == null || nextEvent.Time - clock.CurrentTime > time)
                return;
            if (current.Count != last.Count)
            {
                var timing = ruleset.MapInfo.GetTimingPoint(current.Time);
                var beat = timing.MsPerBeat;
                var fade = beat / 2f;

                using (BeginAbsoluteSequence(current.Time - beat))
                {
                    leftIcon.FadeIn(fade).Then().FadeOut(fade);
                    rightIcon.FadeIn(fade).Then().FadeOut(fade);

                    const float pos = 100f;

                    if (current.Count > last.Count)
                    {
                        leftIcon.RotateTo(0).MoveToX(0)
                                .MoveToX(-pos, beat, Easing.OutQuint);
                        rightIcon.RotateTo(0).MoveToX(0)
                                 .MoveToX(pos, beat, Easing.OutQuint);
                    }
                    else
                    {
                        leftIcon.RotateTo(180).MoveToX(-pos)
                                .MoveToX(0, beat, Easing.OutQuint);
                        rightIcon.RotateTo(180).MoveToX(pos)
                                 .MoveToX(0, beat, Easing.OutQuint);
                    }
                }
            }

            last = current;
        }
    }
}
