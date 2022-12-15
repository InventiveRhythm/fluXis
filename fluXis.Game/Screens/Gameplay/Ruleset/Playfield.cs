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

        public Playfield(GameplayScreen screen)
        {
            Screen = screen;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(1, 1);

            AddInternal(Stage = new Stage());

            for (int i = 0; i < 4; i++)
            {
                Receptor receptor = new Receptor(this, i);
                Receptors.Add(receptor);
                AddInternal(receptor);
            }

            Manager = new HitObjectManager(this);
            TimingLineManager = new TimingLineManager(Manager);

            AddInternal(TimingLineManager);
            AddInternal(Manager);
        }

        public void LoadMap(MapInfo map)
        {
            Manager.LoadMap(map);
            TimingLineManager.CreateLines(map);
        }
    }
}
