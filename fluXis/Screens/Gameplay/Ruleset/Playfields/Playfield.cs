using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Configuration.Experiments;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Screens.Gameplay.Ruleset.Playfields.UI;
using fluXis.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Skinning;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class Playfield : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public int Index { get; }
    public int SubIndex { get; }
    public bool IsSubPlayfield => SubIndex > 0;

    private bool canSeek { get; set; }
    public override bool RemoveCompletedTransforms => !canSeek;

    public float RelativePosition
    {
        get
        {
            var screenWidth = Parent!.DrawWidth;
            return (X + screenWidth / 2) / screenWidth;
        }
    }

    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager HitManager { get; private set; }

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

    public Playfield(int idx, int sub)
    {
        Index = idx;
        SubIndex = sub;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config, ExperimentConfigManager experiments)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;
        Alpha = IsSubPlayfield ? 0 : 1; // we start subfields invisible, so that mappers can decide when they should show

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);
        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);
        hitsoundPanStrength = config.GetBindable<double>(FluXisSetting.HitsoundPanning);

        canSeek = experiments.Get<bool>(ExperimentConfig.Seeking);

        Receptors = new FillFlowContainer<Receptor>
        {
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal,
            ChildrenEnumerable = Enumerable.Range(0, RealmMap.KeyCount).Select(i => new Receptor(i)),
            Padding = new MarginPadding { Bottom = skinManager.SkinJson.GetKeymode(RealmMap.KeyCount).ReceptorOffset }
        };

        dependencies.CacheAs(this);
        dependencies.CacheAs(HitManager = new HitObjectManager
        {
            AlwaysPresent = true,
            Masking = true
        });

        var receptorsFirst = skinManager.SkinJson.GetKeymode(RealmMap.KeyCount).ReceptorsFirst;

        InternalChildren = new[]
        {
            new LaneSwitchAlert(),
            new PulseEffect(),
            stage = new Stage(),
            new TimingLineManager(),
            receptorsFirst ? Receptors : HitManager,
            receptorsFirst ? HitManager : Receptors,
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

        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.HitObjects).ForEach(e => e.Apply(HitManager));
        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Stage).ForEach(e => e.Apply(stage));
        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Receptors).ForEach(e => e.Apply(Receptors));
        screen.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Playfield).ForEach(e => e.Apply(this));

        if (canSeek)
        {
            screen.MapEvents.PlayfieldMoveEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.PlayfieldScaleEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.PlayfieldRotateEvents.ForEach(e => e.Apply(this));
            screen.MapEvents.ScrollMultiplyEvents.ForEach(e => e.Apply(HitManager));
            screen.MapEvents.TimeOffsetEvents.ForEach(e => e.Apply(HitManager));
        }
        else
        {
            AddRangeInternal(new Drawable[]
            {
                new EventHandler<PlayfieldMoveEvent>(screen.MapEvents.PlayfieldMoveEvents),
                new EventHandler<PlayfieldScaleEvent>(screen.MapEvents.PlayfieldScaleEvents),
                new EventHandler<PlayfieldRotateEvent>(screen.MapEvents.PlayfieldRotateEvents),
                new EventHandler<ScrollMultiplierEvent>(screen.MapEvents.ScrollMultiplyEvents),
                new EventHandler<TimeOffsetEvent>(screen.MapEvents.TimeOffsetEvents),
            });
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

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

        if (!IsSubPlayfield)
            screen.Hitsounding.PlayfieldPanning.Value = Math.Clamp(RelativePosition * 2 - 1, -1, 1) * hitsoundPanStrength.Value;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
