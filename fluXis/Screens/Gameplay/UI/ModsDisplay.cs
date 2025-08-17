using System;
using System.Collections.Generic;
using fluXis.Mods;
using fluXis.Mods.Drawables;
using fluXis.Screens.Gameplay.HUD;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.UI;

public partial class ModsDisplay : GameplayHUDComponent
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private GameplayScreen screen { get; set; }

    private ModList modsContainer;

    private List<IMod> mods = null;

    public ModsDisplay(List<IMod> mods = null)
    {
        this.mods = screen?.Mods ?? mods;

        if (this.mods == null) throw new Exception("Mods List is Null for ModsDisplay");
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 80, Right = 20 };
        
        Child = modsContainer = new ModList { Mods = mods };
    }

    protected override void Update()
    {
        if (mods.Count == modsContainer.Count) return;

        modsContainer.ReloadList();
    }
}