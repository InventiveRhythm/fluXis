using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Mods.Drawables;

public partial class ModList : FillFlowContainer<ModIcon>
{
    private IEnumerable<IMod> mods;

    public IEnumerable<IMod> Mods
    {
        get => mods;
        set
        {
            mods = value;

            if (IsLoaded) ReloadList();
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(10);

        InternalChildren = Mods.Select(mod => new ModIcon
        {
            Mod = mod
        }).ToArray();
    }

    public void ReloadList()
    {
        Clear();
        InternalChildren = Mods.Select(mod => new ModIcon
        {
            Mod = mod
        }).ToArray();
    }
}
