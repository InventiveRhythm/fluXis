using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignReceptor : CompositeDrawable
{
    private int idx { get; }

    public EditorDesignReceptor(int idx)
    {
        this.idx = idx;
    }

    [BackgroundDependencyLoader]
    private void load(EditorMap map, SkinManager skinManager)
    {
        Width = 114;
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChild = skinManager.GetReceptor(idx + 1, map.RealmMap.KeyCount, false);
    }
}
