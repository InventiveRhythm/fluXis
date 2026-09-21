using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Mode.Keys.Gameplay;
using fluXis.Mode.Keys.Objects;
using fluXis.Replays;

namespace fluXis.Mode.Keys;

public class KeysAutoGenerator : AutoGenerator
{
    private int mode { get; }
    private bool dual { get; }
    private bool split { get; }

    private List<FluXisGameplayKeybind> keys { get; }

    public KeysAutoGenerator(PlayableMap map)
        : base(map)
    {
        mode = KeysGameMode.ParseMode(map.Mode);
        dual = map.IsDual;
        split = map.IsDualSplit;

        keys = [.. KeysKeybindContainer.GetKeys(mode, dual).Select(x => (FluXisGameplayKeybind)x.Action)];
    }

    protected override IEnumerable<ReplayFrame> GenerateFrames()
    {
        var objs = Map.ObjectsOfType<HitObject>();

        if (objs.Length == 0)
            yield break;

        if (keys.Count <= 0)
            yield break;

        var actions = generateActions().GroupBy(a => a.Time).OrderBy(g => g.First().Time);
        var currentKeys = new List<FluXisGameplayKeybind>();

        foreach (var action in actions)
        {
            foreach (var point in action)
            {
                var key = keys[point.Lane - 1];

                switch (point)
                {
                    case PressAction:
                        currentKeys.Add(key);

                        if (dual && !split)
                            currentKeys.Add(keys[point.Lane - 1 + mode]);

                        break;

                    case ReleaseAction:
                        currentKeys.Remove(key);

                        if (dual && !split)
                            currentKeys.Remove(keys[point.Lane - 1 + mode]);

                        break;
                }
            }

            yield return new ReplayFrame(action.First().Time, [.. currentKeys.Cast<int>()]);
        }
    }

    private IEnumerable<IAction> generateActions()
    {
        var columns = Map.ObjectsOfType<HitObject>().GroupBy(x => x.Lane);

        foreach (var column in columns)
        {
            var objects = column.OrderBy(x => x.Time).ToList();
            var pressed = false;
            double blockedUntil = 0;

            for (int i = 0; i < objects.Count; i++)
            {
                var currentObject = objects[i];
                if (currentObject is Landmine) continue;

                if (currentObject.Time < blockedUntil)
                    continue;

                HitObject? nextObjectInColumn = objects.Count == i + 1 ? null : objects[i + 1];
                var releaseTime = calculateReleaseTime(currentObject, nextObjectInColumn);

                if (!pressed)
                {
                    pressed = true;
                    yield return new PressAction(currentObject.Time, currentObject.Lane);
                }

                if (releaseTime is not null)
                {
                    pressed = false;
                    blockedUntil = releaseTime.Value;
                    yield return new ReleaseAction(releaseTime.Value, currentObject.Lane);
                }
            }
        }
    }

    private double? calculateReleaseTime(HitObject currentObject, HitObject? nextObject)
    {
        var endTime = currentObject.GetEndTime();

        if (currentObject is LongNote)
            return endTime;

        if (nextObject is Tick)
        {
            var diff = nextObject.Time - currentObject.Time;

            if (diff < 200)
                return null;
        }

        var enoughTimeToRelease = nextObject is null || nextObject.Time > endTime + KEY_DOWN_TIME;
        return endTime + (enoughTimeToRelease ? KEY_DOWN_TIME : (nextObject!.Time - endTime) * 0.9f);
    }
}
