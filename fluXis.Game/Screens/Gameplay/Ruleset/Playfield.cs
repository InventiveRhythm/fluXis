using System.Collections.Generic;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset
{
    public class Playfield : CompositeDrawable
    {
        public List<Receptor> Receptors = new List<Receptor>();
        public GameplayScreen Screen;
        public HitObjectManager Manager;
        public TimingLineManager TimingLineManager;
        public Stage Stage;

        private readonly MapInfo map;

        public Playfield(GameplayScreen screen)
        {
            Screen = screen;
            map = screen.Map;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(1, 1);

            Stage = new Stage(map);

            Manager = new HitObjectManager(this);
            Manager.LoadMap(map);

            TimingLineManager = new TimingLineManager(Manager);

            AddInternal(Stage);

            for (int i = 0; i < map.KeyCount; i++)
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
            TimingLineManager.CreateLines(map);
            base.LoadComplete();
        }
    }
}
