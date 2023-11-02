using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Game.Skinning;
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

    public GameplayScreen Screen { get; init; }

    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager Manager { get; private set; }
    public Stage Stage { get; private set; }

    public MapInfo Map => Screen.Map;

    private TimingLineManager timingLineManager;
    private Drawable hitLine;

    private Container laneCovers;
    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;
    private Bindable<ScrollDirection> scrollDirection;

    public bool IsUpScroll => scrollDirection.Value == ScrollDirection.Up;

    private PlayfieldMoveEvent currentPlayfieldMoveEvent;
    private PlayfieldScaleEvent currentPlayfieldScaleEvent;

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

        Stage = new Stage(this);
        Receptors = new FillFlowContainer<Receptor>
        {
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal
        };

        hitLine = skinManager.GetHitLine();
        hitLine.Y = -skinManager.SkinJson.GetKeymode(Map.KeyCount).HitPosition;

        Manager = new HitObjectManager(this);
        Manager.LoadMap(Map);

        timingLineManager = new TimingLineManager(Manager);

        for (int i = 0; i < Map.KeyCount; i++)
        {
            Receptor receptor = new Receptor(i) { Playfield = this };
            Receptors.Add(receptor);
        }

        laneCovers = new Container
        {
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new[]
            {
                topCover = skinManager.GetLaneCover(false),
                bottomCover = skinManager.GetLaneCover(true)
            }
        };

        InternalChildren = new[]
        {
            Stage,
            timingLineManager,
            Manager,
            Receptors,
            hitLine,
            laneCovers
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
        hitLine.Width = Stage.Width;
        laneCovers.Width = Stage.Width;

        topCover.Y = (topCoverHeight.Value - 1f) / 2f;
        bottomCover.Y = (1f - bottomCoverHeight.Value) / 2f;

        updateOffset();
        updateScale();
    }

    private void updateScale()
    {
        var ev = currentPlayfieldScaleEvent;

        foreach (var psEvent in Screen.MapEvents.PlayfieldScaleEvents)
        {
            if (psEvent.Time <= Clock.CurrentTime)
                currentPlayfieldScaleEvent = psEvent;
        }

        if (ev == currentPlayfieldScaleEvent) return;

        var yScale = currentPlayfieldScaleEvent.ScaleY;
        if (IsUpScroll) yScale *= -1;

        this.ScaleTo(new Vector2(currentPlayfieldScaleEvent.ScaleX, yScale), currentPlayfieldScaleEvent.Duration, currentPlayfieldScaleEvent.Easing);
    }

    private void updateOffset()
    {
        var ev = currentPlayfieldMoveEvent;

        foreach (var pmEvent in Screen.MapEvents.PlayfieldMoveEvents)
        {
            if (pmEvent.Time <= Clock.CurrentTime) currentPlayfieldMoveEvent = pmEvent;
        }

        if (ev != currentPlayfieldMoveEvent)
            this.MoveToX(currentPlayfieldMoveEvent.OffsetX, currentPlayfieldMoveEvent.Duration, Easing.OutQuint);
    }
}
