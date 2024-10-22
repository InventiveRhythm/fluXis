using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Replays;

namespace fluXis.Game.Replays;

public class AutoGenerator
{
    /// <summary>
    /// How long the key is held down for
    /// </summary>
    private const float key_down_time = 50;

    private MapInfo map { get; }
    private int mode { get; }
    private bool dual { get; }

    private List<FluXisGameplayKeybind> keys { get; }

    private List<ReplayFrame> frames { get; } = new();

    public AutoGenerator(MapInfo map, int mode)
    {
        this.map = map;
        this.mode = mode;
        dual = map.IsDual;

        keys = GameplayInput.GetKeys(mode, dual).ToList();
    }

    public Replay Generate()
    {
        frames.Clear();
        generateFrames();

        var replay = new Replay
        {
            PlayerID = APIUser.AutoPlay.ID,
            Frames = frames
        };

        return replay;
    }

    private void generateFrames()
    {
        if (map.HitObjects.Count == 0)
            return;

        if (keys.Count <= 0)
            return;

        var actions = generateActions().GroupBy(a => a.Time).OrderBy(g => g.First().Time);

        var currentKeys = new List<FluXisGameplayKeybind>();

        foreach (var action in actions)
        {
            foreach (var point in action)
            {
                switch (point)
                {
                    case PressAction:
                        currentKeys.Add(keys[point.Lane - 1]);

                        if (dual)
                            currentKeys.Add(keys[point.Lane - 1 + mode]);

                        break;

                    case ReleaseAction:
                        currentKeys.Remove(keys[point.Lane - 1]);

                        if (dual)
                            currentKeys.Remove(keys[point.Lane - 1 + mode]);

                        break;
                }
            }

            frames.Add(new ReplayFrame(action.First().Time, currentKeys.Cast<int>().ToArray()));
        }
    }

    private IEnumerable<IAction> generateActions()
    {
        for (int i = 0; i < map.HitObjects.Count; i++)
        {
            var currentObject = map.HitObjects[i];
            var nextObjectInColumn = getNextObject(i);
            var releaseTime = calculateReleaseTime(currentObject, nextObjectInColumn);

            yield return new PressAction { Time = currentObject.Time, Lane = currentObject.Lane };
            yield return new ReleaseAction { Time = releaseTime, Lane = currentObject.Lane };
        }
    }

    private HitObject getNextObject(int currentIndex)
    {
        var desiredColumn = map.HitObjects[currentIndex].Lane;

        for (var i = currentIndex + 1; i < map.HitObjects.Count; i++)
        {
            if (map.HitObjects[i].Lane == desiredColumn)
                return map.HitObjects[i];
        }

        return null;
    }

    private double calculateReleaseTime(HitObject currentObject, HitObject nextObject)
    {
        var endTime = currentObject.EndTime;

        if (currentObject.LongNote)
            return endTime;

        var canDelayKeyUpFully = nextObject == null || nextObject.Time > endTime + key_down_time;
        return endTime + (canDelayKeyUpFully ? key_down_time : (nextObject.Time - endTime) * 0.9f);
    }

    private interface IAction
    {
        double Time { get; set; }
        int Lane { get; set; }
    }

    private class PressAction : IAction
    {
        public double Time { get; set; }
        public int Lane { get; set; }
    }

    private class ReleaseAction : IAction
    {
        public double Time { get; set; }
        public int Lane { get; set; }
    }
}
