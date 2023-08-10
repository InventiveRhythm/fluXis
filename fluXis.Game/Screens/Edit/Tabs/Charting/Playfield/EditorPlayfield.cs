using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect;
using fluXis.Game.Screens.Edit.Tabs.Charting.Lines;
using fluXis.Game.Skinning.Default.Stage;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

[Cached]
public partial class EditorPlayfield : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    public EditorHitObjectContainer HitObjectContainer { get; private set; }
    private EditorTimingLines timingLines;

    private HitObjectInfo placementObject;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorHitObjectContainer.NOTEWIDTH * values.MapInfo.KeyCount;
        RelativeSizeAxes = Axes.Y;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new DefaultStageBackground(),
            new DefaultStageBorderLeft(),
            new DefaultStageBorderRight(),
            new EditorEffectContainer(),
            timingLines = new EditorTimingLines(),
            HitObjectContainer = new EditorHitObjectContainer()
        };
    }

    public void StartPlacement(HitObjectInfo hitObject)
    {
        placementObject = hitObject;
    }

    public void FinishPlacement(HitObjectInfo hitObject, bool place)
    {
        if (place)
            values.MapInfo.Add(hitObject);
    }
}
