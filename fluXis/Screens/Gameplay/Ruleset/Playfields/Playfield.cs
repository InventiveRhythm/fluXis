using System;
using System.Linq;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
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
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private Hitsounding hitsounding { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    public int Index { get; }
    public int SubIndex { get; }
    public bool IsSubPlayfield => SubIndex > 0;

    public override bool RemoveCompletedTransforms => false;

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

    public MapInfo MapInfo => ruleset.MapInfo;
    public MapEvents MapEvents => ruleset.MapEvents;
    public RealmMap RealmMap => MapInfo.RealmEntry!;

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
    private void load(FluXisConfig config)
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
            new EventHandler<ShakeEvent>(MapEvents.ShakeEvents, shake => ruleset.ShakeTarget.Shake(shake.Duration, shake.Magnitude))
        };

        MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.HitObjects).ForEach(e => e.Apply(HitManager));
        MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Stage).ForEach(e => e.Apply(stage));
        MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Receptors).ForEach(e => e.Apply(Receptors));
        MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Playfield).ForEach(e => e.Apply(this));

        MapEvents.PlayfieldMoveEvents.ForEach(e => e.Apply(this));
        MapEvents.PlayfieldScaleEvents.ForEach(e => e.Apply(this));
        MapEvents.PlayfieldRotateEvents.ForEach(e => e.Apply(this));
        MapEvents.ScrollMultiplyEvents.ForEach(e => e.Apply(HitManager));
        MapEvents.TimeOffsetEvents.ForEach(e => e.Apply(HitManager));
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
            hitsounding.PlayfieldPanning.Value = Math.Clamp(RelativePosition * 2 - 1, -1, 1) * hitsoundPanStrength.Value;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
