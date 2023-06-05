using fluXis.Game.Configuration;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Playfield : CompositeDrawable
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    public FillFlowContainer<Receptor> Receptors;
    public GameplayScreen Screen;
    public HitObjectManager Manager;
    public TimingLineManager TimingLineManager;
    public Stage Stage;
    public Drawable HitLine;

    private Container laneCovers;
    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;

    public MapInfo Map { get; }

    public Playfield(GameplayScreen screen)
    {
        Screen = screen;
        Map = screen.Map;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Stage = new Stage(this);
        Receptors = new FillFlowContainer<Receptor>
        {
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal,
        };

        HitLine = skinManager.GetHitLine();
        HitLine.Y = -skinManager.CurrentSkin.HitPosition;

        Manager = new HitObjectManager(this);
        Manager.LoadMap(Map);

        TimingLineManager = new TimingLineManager(Manager);

        for (int i = 0; i < Map.KeyCount; i++)
        {
            Receptor receptor = new Receptor(this, i);
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

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);

        InternalChildren = new[]
        {
            Stage,
            TimingLineManager,
            Manager,
            Receptors,
            HitLine,
            laneCovers
        };
    }

    protected override void LoadComplete()
    {
        TimingLineManager.CreateLines(Map);
        base.LoadComplete();
    }

    protected override void Update()
    {
        HitLine.Width = Stage.Width;
        laneCovers.Width = Stage.Width;

        topCover.Y = (topCoverHeight.Value - 1f) / 2f;
        bottomCover.Y = (1f - bottomCoverHeight.Value) / 2f;
    }
}
