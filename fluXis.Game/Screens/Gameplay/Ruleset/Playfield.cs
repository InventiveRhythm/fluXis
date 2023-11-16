using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Game.Skinning;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Playfield : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager Manager { get; private set; }
    public Stage Stage { get; private set; }

    public MapInfo Map => screen.Map;
    public RealmMap RealmMap => screen.RealmMap;

    private TimingLineManager timingLineManager;
    private Drawable hitLine;

    private Container laneCovers;
    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;
    private Bindable<ScrollDirection> scrollDirection;

    public bool IsUpScroll => scrollDirection.Value == ScrollDirection.Up;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);
        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);

        InternalChildren = new[]
        {
            Stage = new Stage(),
            timingLineManager = new TimingLineManager(),
            Manager = new HitObjectManager(),
            Receptors = new FillFlowContainer<Receptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                ChildrenEnumerable = Enumerable.Range(0, RealmMap.KeyCount).Select(i => new Receptor(i))
            },
            hitLine = skinManager.GetHitLine().With(d =>
            {
                d.RelativeSizeAxes = Axes.X;
                d.Y = -skinManager.SkinJson.GetKeymode(RealmMap.KeyCount).HitPosition;
            }),
            laneCovers = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new[]
                {
                    topCover = skinManager.GetLaneCover(false),
                    bottomCover = skinManager.GetLaneCover(true)
                }
            },
            new EventHandler<PlayfieldMoveEvent>(screen.MapEvents.PlayfieldMoveEvents, move => this.MoveToX(move.OffsetX, move.Duration, move.Easing)),
            new EventHandler<PlayfieldScaleEvent>(screen.MapEvents.PlayfieldScaleEvents, scale =>
            {
                var yScale = scale.ScaleY;
                if (IsUpScroll) yScale *= -1;

                this.ScaleTo(new Vector2(scale.ScaleX, yScale), scale.Duration, scale.Easing);
            }),
            new EventHandler<ShakeEvent>(screen.MapEvents.ShakeEvents, shake => screen.Shake(shake.Duration, shake.Magnitude))
        };
    }

    protected override void LoadComplete()
    {
        timingLineManager.CreateLines(Map);
        base.LoadComplete();

        scrollDirection.BindValueChanged(_ =>
        {
            if (IsUpScroll)
                Scale *= new Vector2(1, -1);
        }, true);
    }

    protected override void Update()
    {
        topCover.Y = (topCoverHeight.Value - 1f) / 2f;
        bottomCover.Y = (1f - bottomCoverHeight.Value) / 2f;
    }
}
