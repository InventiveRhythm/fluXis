using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Scoring.Processing;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.UI;

public partial class ComboBurst : CompositeDrawable
{
    [Resolved]
    private IBeatSyncProvider beatSync { get; set; }

    private readonly RulesetContainer ruleset;
    private ScoreProcessor score => ruleset.PlayfieldManager.FirstPlayer.ScoreProcessor;

    private List<Sample> samples;
    private int lastIdx;
    private double lastTime;

    public ComboBurst(RulesetContainer ruleset)
    {
        this.ruleset = ruleset;
    }

    [BackgroundDependencyLoader]
    private void load(ISkin skin)
    {
        RelativeSizeAxes = Axes.Both;

        skin.GetComboBursts().ForEach(x =>
        {
            x.Anchor = Anchor.CentreRight;
            x.Origin = Anchor.CentreLeft;
            x.AlwaysPresent = true;
            x.X = 1;

            AddInternal(x);
        });

        samples = skin.GetComboBurstSamples().ToList();
    }

    protected override void Update()
    {
        base.Update();

        var idx = (int)Math.Floor(score.Combo.Value / 100f);
        if (idx == lastIdx) return;

        lastIdx = idx;
        if (idx == 0) return;

        var beat = beatSync.BeatTime;

        if (Time.Current - lastTime <= beat * 16)
            return;

        lastTime = Time.Current;

        if (InternalChildren.Count > 0)
        {
            var child = InternalChildren[idx % InternalChildren.Count];

            child.FadeIn().MoveToX(-child.DrawWidth, beat * 4, Easing.OutQuint)
                 .Then().FadeOut(beat).Then(beat).MoveToX(0);
        }

        if (samples.Count > 0)
            samples[idx % samples.Count].Play();
    }
}
