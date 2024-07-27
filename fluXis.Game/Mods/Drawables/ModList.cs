using System;
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

    public bool AlwaysRefresh { get; set; } = false;

    public List<IMod> Mods
    {
        get => mods;
        set
        {
            if (mods == null || value == null)
                return;

            value.RemoveAll(mod => mod is null);

            value.Sort((a, b) => string.Compare(a.Acronym, b.Acronym, StringComparison.Ordinal));

            var current = mods.Select(mod => mod.Acronym).ToList();
            var valueList = value.Select(mod => mod.Acronym).ToList();

            if (current.SequenceEqual(valueList) && !AlwaysRefresh)
                return;

            mods.RemoveAll(mod => mod is null);

            mods = value;

            if (IsLoaded) ReloadList();
        }
    }

    public int ModSpacing { get; set; } = 10;

    private Action<ModIcon> createIcon { get; }

    public ModList(Action<ModIcon> createIcon = null)
    {
        AutoSizeAxes = Axes.Both;
        this.createIcon = createIcon;
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
        InternalChildren = Mods.Select(mod =>
        {
            var icon = new ModIcon { Mod = mod };
            createIcon?.Invoke(icon);
            return icon;
        }).ToArray();
    }
}
