using System.Linq;
using fluXis.Game.Screens.Edit.Tabs.Shared.Lines;
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

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;

        InternalChildren = new[]
        {
            skinManager.GetStageBackground(),
            skinManager.GetStageBorder(false),
            skinManager.GetStageBorder(true),
            new EditorTimingLines(),
            Receptors = new FillFlowContainer<EditorDesignReceptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        reload();
    }

    private void reload()
    {
        Receptors.Clear();
        Receptors.ChildrenEnumerable = Enumerable.Range(0, map.RealmMap.KeyCount).Select(i => new EditorDesignReceptor(i));
    }

    protected override void Update()
    {
        base.Update();

        updateScale();
        updatePosition();
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
