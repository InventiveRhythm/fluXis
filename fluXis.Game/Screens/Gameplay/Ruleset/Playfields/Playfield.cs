using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields.UI;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Game.Skinning;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset.Playfields;

public partial class Playfield : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private GameplaySamples samples { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public int Index { get; }
    private bool canSeek { get; }
    public override bool RemoveCompletedTransforms => !canSeek;

    public float RelativePosition
    {
        get
        {
            var screenWidth = Parent!.DrawWidth;
            return (X + screenWidth / 2) / screenWidth;
        }
    }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager Manager { get; private set; }

    public MapInfo Map => screen.Map;
    public RealmMap RealmMap => screen.RealmMap;

    private DependencyContainer dependencies;

    private Stage stage;
    private Drawable hitline;
    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;
    private Bindable<ScrollDirection> scrollDirection;
    private Bindable<double> hitsoundPanStrength;

    public bool IsUpScroll => scrollDirection.Value == ScrollDirection.Up;

    public Playfield(int idx, bool canSeek)
    {
        Index = idx;
        this.canSeek = canSeek;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);
        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);
        hitsoundPanStrength = config.GetBindable<double>(FluXisSetting.HitsoundPanning);

        JudgementProcessor.AddDependants(new JudgementDependant[]
        {
            HealthProcessor = screen.CreateHealthProcessor(),
            ScoreProcessor = new ScoreProcessor
            {
                Player = screen.CurrentPlayer ?? APIUser.Default,
                HitWindows = screen.HitWindows,
                Map = RealmMap,
                MapInfo = Map,
                Mods = screen.Mods
            }
        });

        dependencies.CacheAs(this);
        dependencies.CacheAs(Manager = new HitObjectManager
        {
            AlwaysPresent = true,
            Masking = true
        });

        InternalChildren = new[]
        {
            new LaneSwitchAlert(),
            new PulseEffect(),
            stage = new Stage(),
            new TimingLineManager(),
            Manager,
            Receptors = new FillFlowContainer<Receptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                ChildrenEnumerable = Enumerable.Range(0, RealmMap.KeyCount).Select(i => new Receptor(i))
            },
            hitline = skinManager.GetHitLine().With(d =>
            {
                d.Width = 1;
                d.RelativeSizeAxes = Axes.X;
            }),
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Children = new[]
                {
                    topCover = skinManager.GetLaneCover(false),
                    bottomCover = skinManager.GetLaneCover(true)
                }
            },
            new KeyOverlay(),
            new EventHandler<ShakeEvent>(screen.MapEvents.ShakeEvents, shake => screen.Shake(shake.Duration, shake.Magnitude))
        };

        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.HitObjects).ForEach(e => e.Apply(Manager));
        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Stage).ForEach(e => e.Apply(stage));
        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Receptors).ForEach(e => e.Apply(Receptors));

        if (canSeek)
        {
            screen.MapEvents.PlayfieldMoveEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.PlayfieldScaleEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.PlayfieldFadeEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.PlayfieldRotateEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.ScrollMultiplyEvents.ForEach(e => e.Apply(Manager));
            screen.MapEvents.TimeOffsetEvents.ForEach(e => e.Apply(Manager));
        }
        else
        {
            AddRangeInternal(new Drawable[]
            {
                new EventHandler<PlayfieldMoveEvent>(screen.MapEvents.PlayfieldMoveEvents),
                new EventHandler<PlayfieldScaleEvent>(screen.MapEvents.PlayfieldScaleEvents),
                new EventHandler<PlayfieldFadeEvent>(screen.MapEvents.PlayfieldFadeEvents),
                new EventHandler<PlayfieldRotateEvent>(screen.MapEvents.PlayfieldRotateEvents),
                new EventHandler<ScrollMultiplierEvent>(screen.MapEvents.ScrollMultiplyEvents),
                new EventHandler<TimeOffsetEvent>(screen.MapEvents.TimeOffsetEvents),
            });
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        JudgementProcessor.ApplyMap(Map);
        ScoreProcessor.OnComboBreak += samples.Miss;

        scrollDirection.BindValueChanged(_ =>
        {
            if (IsUpScroll)
                Scale *= new Vector2(1, -1);
        }, true);
    }

    protected override void Update()
    {
        hitline.Y = -laneSwitchManager.HitPosition;

        topCover.Y = (topCoverHeight.Value - 1f) / 2f;
        bottomCover.Y = (1f - bottomCoverHeight.Value) / 2f;

        screen.Hitsounding.PlayfieldPanning.Value = Math.Clamp(RelativePosition * 2 - 1, -1, 1) * hitsoundPanStrength.Value;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
