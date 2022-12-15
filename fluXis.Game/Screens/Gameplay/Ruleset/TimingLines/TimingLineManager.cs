using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines
{
    public class TimingLineManager : CompositeDrawable
    {
        public HitObjectManager HitObjectManager { get; }

        public TimingLineManager(HitObjectManager hitObjectManager)
        {
            HitObjectManager = hitObjectManager;
            Width = HitObjectManager.Playfield.Stage.Background.Width;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        public void CreateLines(MapInfo map)
        {
            for (int i = 0; i < map.TimingPoints.Count; i++)
            {
                var point = map.TimingPoints[i];
                int target = i + 1 < map.TimingPoints.Count ? map.TimingPoints[i + 1].Time : map.EndTime;
                int increase = point.Signature * point.GetMsPerBeat();
                int position = point.Time;

                while (position < target)
                {
                    AddInternal(new TimingLine(this, position));
                    position += increase;
                }
            }
        }
    }
}
