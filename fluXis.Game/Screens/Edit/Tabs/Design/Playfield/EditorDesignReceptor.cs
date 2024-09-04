using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignReceptor : CompositeDrawable
{
    private int idx { get; }
    private LaneSwitchManager manager { get; }

    public EditorDesignReceptor(int idx, LaneSwitchManager manager)
    {
        this.idx = idx;
        this.manager = manager;
    }

    [BackgroundDependencyLoader]
    private void load(EditorMap map, SkinManager skinManager)
    {
        Width = 114;
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        InternalChild = skinManager.GetReceptor(idx + 1, map.RealmMap.KeyCount, false);
    }

    protected override void Update()
    {
        base.Update();
        Width = manager.WidthFor(idx + 1);
    }
}
