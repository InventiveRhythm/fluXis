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
            if (mods == null)
                return;

            var current = mods.Select(mod => mod.Acronym).ToList();
            var valueList = value.Select(mod => mod.Acronym).ToList();

            if (current.SequenceEqual(valueList))
                return;

            mods.RemoveAll(mod => mod is null);

            mods = value;

            if (IsLoaded) ReloadList();
        }
    }

    public int ModSpacing { get; set; } = 10;

    public ModList()
    {
        AutoSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
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
