using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Mode.Keys.Gameplay.Objects;

public partial class KeysHitObjectColumn : KeysHitObjectManager, IKeyBindingHandler<FluXisGameplayKeybind>
{
    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; } = null!;

    /// <summary>
    /// one-based index defining the lane of the column
    /// </summary>
    public int Index { get; set; }

    private FluXisGameplayKeybind action;

    public override float HitPosition => hitPosition;
    private float hitPosition;

    public KeysHitObjectColumn(RulesetContainer ruleset, PlayableMap map, int idx, IEnumerable<HitObject> objs)
        : base(ruleset, map, objs, true)
    {
        Index = idx;
        DefaultScrollGroup = ruleset.ScrollGroups[$"${Index}"];
    }

    [BackgroundDependencyLoader]
    private void load(KeysKeybindContainer keybinds)
    {
        action = keybinds.Keys[Math.Clamp(Index - 1, 0, keybinds.Keys.Length - 1)];
    }

    protected override void Update()
    {
        base.Update();

        Width = laneSwitchManager.WidthFor(Index);
        X = laneSwitchManager.PositionOf(Index);
        hitPosition = DrawHeight - laneSwitchManager.HitPosition;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
    {
        if (e.Repeat || e.Action != action)
            return false;

        HitObject? obj = ActiveObjects.Count > 0 ? ActiveObjects.MinBy(x => x.Object.Time)?.Object : null;
        obj ??= FutureObjects.FirstOrDefault();
        if (obj is null) return false;

        PlayObjectSample(obj, false);
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e)
    {
    }
}
