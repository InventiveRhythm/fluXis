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
            HitObjectContainer = new EditorHitObjectContainer()
        };
    }
}
