﻿using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class PlayfieldPlayer : CompositeDrawable, IHUDDependencyProvider
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GameplaySamples samples { get; set; }

    [Resolved]
    private RulesetContainer ruleset { get; set; }

    public Playfield MainPlayfield { get; }
    public Playfield[] SubPlayfields { get; }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    private DependencyContainer dependencies;

    public PlayfieldPlayer(int index, int subCount)
    {
        MainPlayfield = new Playfield(index, 0);
        SubPlayfields = Enumerable.Range(1, subCount).Select(x => new Playfield(index, x)).ToArray();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        dependencies.CacheAs(this);

        JudgementProcessor.AddDependants(new JudgementDependant[]
        {
            HealthProcessor = ruleset.CreateHealthProcessor(),
            ScoreProcessor = new ScoreProcessor
            {
                Player = ruleset.CurrentPlayer ?? APIUser.Default,
                HitWindows = ruleset.HitWindows,
                MapInfo = ruleset.MapInfo,
                Mods = ruleset.Mods
            }
        });

        AddInternal(dependencies.CacheAsAndReturn(new LaneSwitchManager(ruleset.MapEvents.LaneSwitchEvents, ruleset.MapInfo.RealmEntry!.KeyCount, ruleset.MapInfo.NewLaneSwitchLayout, ruleset.Mods.Any(x => x is MirrorMod))));

        var content = new SortingContainer { RelativeSizeAxes = Axes.Both };
        content.Child = MainPlayfield;
        content.AddRange(SubPlayfields);
        AddInternal(content);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        JudgementProcessor.ApplyMap(ruleset.MapInfo);
        HealthProcessor.OnSavedDeath += () => samples?.EarlyFail();
        ScoreProcessor.OnComboBreak += () =>
        {
            if (ruleset.CatchingUp)
                return;

            samples?.Miss();
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    private partial class SortingContainer : Container<Playfield>
    {
        protected override int Compare(Drawable x, Drawable y)
        {
            var a = (Playfield)x;
            var b = (Playfield)y;

            var result = -a.AnimationZ.CompareTo(b.AnimationZ);

            if (result != 0)
                return result;

            return -a.SubIndex.CompareTo(b.SubIndex);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            SortInternal();
        }
    }

    #region IHUDDependencyProvider

    HitWindows IHUDDependencyProvider.HitWindows => ruleset.HitWindows;
    RealmMap IHUDDependencyProvider.RealmMap => ruleset.MapInfo.RealmEntry;
    MapInfo IHUDDependencyProvider.MapInfo => ruleset.MapInfo;
    float IHUDDependencyProvider.PlaybackRate => ruleset.Rate;
    double IHUDDependencyProvider.CurrentTime => ruleset.Time.Current;

    #endregion
}
