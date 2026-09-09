using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Modes.Gameplay.Objects;
using fluXis.Modes.Keys.Map.Objects.Drawable;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Modes.Keys.Gameplay;

#nullable enable

public partial class KeysHitObjectManager : GameModeHitObjectManager
{
    protected ScrollGroup DefaultScrollGroup { get; init; }

    public float ScrollSpeed
    {
        get
        {
            var speed = Playfield.RealmMap.Settings.ScrollSpeed ?? Ruleset.ScrollSpeed.Value;
            return speed / Ruleset.Rate;
        }
    }

    public virtual float HitPosition => DrawHeight;
    public double VisualTimeOffset { get; set; } = 0;

    public int KeyCount => Map.RealmEntry!.KeyCount;

    public KeysHitObjectManager(RulesetContainer ruleset, MapInfo map, MapEvents events, IEnumerable<HitObject> objs, bool nested = false)
        : base(ruleset, map, events)
    {
        RelativeSizeAxes = Axes.Y;
        DefaultScrollGroup = ruleset.ScrollGroups["$1"];

        if (nested)
            FutureObjects.AddRange(objs.OrderBy(x => x.Time));
        else
        {
            RelativeSizeAxes |= Axes.X;

            var grouped = objs.GroupBy(x => x.Lane).OrderBy(x => x.Key).ToArray();
            grouped.ForEach(g =>
            {
                var manager = new KeysHitObjectColumn(ruleset, Map, Events, g.Key, g);
                AddNestedManager(manager);
                AddInternal(manager);
            });
        }
    }

    protected override bool ShouldBeRendered(HitObject obj)
    {
        var group = obj.ScrollGroup ?? DefaultScrollGroup;
        var svTime = group.PositionFromTime(obj.Time);
        var y = PositionAtTime(svTime, group);
        return y >= 0;
    }

    protected override DrawableHitObject? CreateDrawableFor(HitObject obj)
    {
        if (obj is { Type: HitObjectType.Normal, LongNote: false })
            return new DrawableNote(obj);

        return null;
    }

    #region Positioning

    public float PositionAtTime(double time, ScrollGroup? group = null, Easing ease = Easing.None)
    {
        group ??= DefaultScrollGroup;

        var pos = HitPosition;
        var current = group.CurrentTime + VisualTimeOffset;
        var y = (float)(pos - .5f * ((time - (float)current) * (ScrollSpeed * group.ScrollMultiplier)));

        if (ease <= Easing.None || y < 0 || y > pos)
            return y;

        var progress = y / pos;
        y = Interpolation.ValueAt(progress, 0, pos, 0, 1, ease);
        return float.IsFinite(y) ? y : 0;
    }

    #endregion
}
