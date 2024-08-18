using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Audio;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay;

public partial class AnimatedEventHandler<T> : CompositeComponent
    where T : ITimedObject
{
    [Resolved]
    private GameplayClock clock { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    private List<T> objects { get; }

    private bool appliesToPlayfield { get; }

    public AnimatedEventHandler(IEnumerable<T> objects)
    {
        this.objects = objects.ToList();
        this.objects.Sort((a, b) => a.Time.CompareTo(b.Time));

        var first = objects.FirstOrDefault();
        appliesToPlayfield = first is IApplicableToPlayfield;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        setupTransforms();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        clock.OnSeek += (prev, now) =>
        {
            if (appliesToPlayfield)
            {
                playfield.ClearTransforms();
                setupTransforms();
            }
        };
    }

    private void setupDefaults()
    {
        if (appliesToPlayfield)
        {
            using (playfield.BeginAbsoluteSequence(-20000))
            {
                playfield.ScaleTo(1);
                playfield.MoveTo(Vector2.Zero);
                playfield.FadeIn();
                playfield.RotateTo(0);
            }
        }
    }

    private void setupTransforms()
    {
        setupDefaults();

        foreach (var o in objects)
        {
            if (o is IApplicableToPlayfield p)
                p.Apply(playfield);
        }
    }
}
