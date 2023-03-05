using System.Collections.Generic;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Playfield : CompositeDrawable
{
    public List<Receptor> Receptors = new();
    public GameplayScreen Screen;
    public HitObjectManager Manager;
    public TimingLineManager TimingLineManager;
    public Stage Stage;

    public MapInfo Map { get; }

    public Playfield(GameplayScreen screen)
    {
        Screen = screen;
        Map = screen.Map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Stage = new Stage(this);

        Manager = new HitObjectManager(this);
        Manager.LoadMap(Map);

        TimingLineManager = new TimingLineManager(Manager);

        AddInternal(Stage);

        for (int i = 0; i < Map.KeyCount; i++)
        {
            Receptor receptor = new Receptor(this, i);
            Receptors.Add(receptor);
            AddInternal(receptor);
        }

        AddInternal(TimingLineManager);
        AddInternal(Manager);
    }

    protected override void LoadComplete()
    {
        TimingLineManager.CreateLines(Map);
        base.LoadComplete();
    }
}