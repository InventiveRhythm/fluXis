using System.Linq;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Lines;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignPlayfield : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public FillFlowContainer<EditorDesignReceptor> Receptors { get; private set; }
    private Drawable hitline;

    private LaneSwitchManager laneSwitchManager;

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;

        laneSwitchManager = new LaneSwitchManager(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount)
        {
            KeepTransforms = true,
            Clock = clock
        };

        InternalChildren = new[]
        {
            laneSwitchManager,
            new Stage(),
            new EditorTimingLines(),
            Receptors = new FillFlowContainer<EditorDesignReceptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal
            },
            hitline = skinManager.GetHitLine().With(l =>
            {
                l.RelativeSizeAxes = Axes.X;
                l.Width = 1;
            })
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.LaneSwitchEventAdded += reloadLaneSwitches;
        map.LaneSwitchEventUpdated += reloadLaneSwitches;
        map.LaneSwitchEventRemoved += reloadLaneSwitches;

        reload();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        map.LaneSwitchEventAdded -= reloadLaneSwitches;
        map.LaneSwitchEventUpdated -= reloadLaneSwitches;
        map.LaneSwitchEventRemoved -= reloadLaneSwitches;
    }

    private void reload()
    {
        reloadLaneSwitches(null);
        Receptors.Clear();
        Receptors.ChildrenEnumerable = Enumerable.Range(0, map.RealmMap.KeyCount).Select(i => new EditorDesignReceptor(i, laneSwitchManager));
    }

    private void reloadLaneSwitches(LaneSwitchEvent _)
    {
        laneSwitchManager.Rebuild(map.MapEvents.LaneSwitchEvents, map.RealmMap.KeyCount);
    }

    protected override void Update()
    {
        base.Update();

        hitline.Y = -laneSwitchManager.HitPosition;

        updateRotation();
        updateScale();
        updatePosition();
        updateAlpha();
    }

    private void updateRotation()
    {
        var current = map.MapEvents.PlayfieldRotateEvents.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            Rotation = 0;
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var end = current.Roll;

        if (progress >= 1)
        {
            Rotation = end;
            return;
        }

        var previous = map.MapEvents.PlayfieldRotateEvents.LastOrDefault(e => e.Time < current.Time);
        var start = previous?.Roll ?? 0;

        if (progress < 0)
        {
            Rotation = start;
            return;
        }

        Rotation = Interpolation.ValueAt(clock.CurrentTime, start, end, current.Time, current.Time + current.Duration, current.Easing);
    }

    private void updateAlpha()
    {
        var current = map.MapEvents.PlayfieldFadeEvents.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            Alpha = 1;
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var end = current.Alpha;

        if (progress >= 1)
        {
            Alpha = end;
            return;
        }

        var previous = map.MapEvents.PlayfieldFadeEvents.LastOrDefault(e => e.Time < current.Time);
        var start = previous?.Alpha ?? 1;

        if (progress < 0)
        {
            Alpha = start;
            return;
        }

        Alpha = Interpolation.ValueAt(clock.CurrentTime, start, end, current.Time, current.Time + current.Duration, current.Easing);
    }

    private void updateScale()
    {
        var curScale = map.MapEvents.PlayfieldScaleEvents.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (curScale == null)
        {
            Scale = Vector2.One;
            return;
        }

        var progress = (clock.CurrentTime - curScale.Time) / curScale.Duration;
        var end = new Vector2(curScale.ScaleX, curScale.ScaleY);

        if (progress >= 1)
        {
            Scale = end;
            return;
        }

        var prevScale = map.MapEvents.PlayfieldScaleEvents.LastOrDefault(e => e.Time < curScale.Time);
        var prev = Vector2.One;

        if (prevScale != null)
            prev = new Vector2(prevScale.ScaleX, prevScale.ScaleY);

        if (progress < 0)
        {
            Scale = prev;
            return;
        }

        Scale = Interpolation.ValueAt(clock.CurrentTime, prev, end, curScale.Time, curScale.Time + curScale.Duration, curScale.Easing);
    }

    private void updatePosition()
    {
        var curMove = map.MapEvents.PlayfieldMoveEvents.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (curMove == null)
        {
            Position = Vector2.One;
            return;
        }

        var progress = (clock.CurrentTime - curMove.Time) / curMove.Duration;
        var end = new Vector2(curMove.OffsetX, curMove.OffsetY);

        if (progress >= 1)
        {
            Position = end;
            return;
        }

        var prevMove = map.MapEvents.PlayfieldMoveEvents.LastOrDefault(e => e.Time < curMove.Time);
        var prev = Vector2.One;

        if (prevMove != null)
            prev = new Vector2(prevMove.OffsetX, prevMove.OffsetY);

        if (progress < 0)
        {
            Position = prev;
            return;
        }

        Position = Interpolation.ValueAt(clock.CurrentTime, prev, end, curMove.Time, curMove.Time + curMove.Duration, curMove.Easing);
    }
}
