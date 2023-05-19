using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Playfield : CompositeDrawable
{
    public FillFlowContainer<Receptor> Receptors;
    public GameplayScreen Screen;
    public HitObjectManager Manager;
    public TimingLineManager TimingLineManager;
    public Stage Stage;
    public Box HitLine;

    public MapInfo Map { get; }

    public Playfield(GameplayScreen screen)
    {
        Screen = screen;
        Map = screen.Map;

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

        HitLine = new Box
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.TopCentre,
            Height = 3,
            Colour = ColourInfo.GradientHorizontal(FluXisColors.Accent3, FluXisColors.Accent)
        };

        Manager = new HitObjectManager(this);
        Manager.LoadMap(Map);

        TimingLineManager = new TimingLineManager(Manager);

        for (int i = 0; i < Map.KeyCount; i++)
        {
            Receptor receptor = new Receptor(this, i);
            Receptors.Add(receptor);
        }

        InternalChildren = new Drawable[]
        {
            Stage,
            TimingLineManager,
            Manager,
            Receptors,
            HitLine
        };
    }

    protected override void LoadComplete()
    {
        TimingLineManager.CreateLines(Map);
        base.LoadComplete();
    }

    protected override void Update()
    {
        HitLine.Y = -Receptors.First().HitPosition;
        HitLine.Width = Stage.Background.Width;
    }
}
