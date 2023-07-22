using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Mods.Drawables;

public partial class ModList : FillFlowContainer<ModIcon>
{
    private List<IMod> mods = new();

    public List<IMod> Mods
    {
        get => mods;
        set
        {
            mods = value;

            if (IsLoaded) ReloadList();
        }
    }

    public int ModSpacing { get; set; } = 10;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(ModSpacing);

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
