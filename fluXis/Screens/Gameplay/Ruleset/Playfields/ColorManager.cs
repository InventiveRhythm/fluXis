using System;
using System.Collections.Generic;
using fluXis.Map;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class ColorManager : Component, ICustomColorProvider
{
    [Resolved]
    private MapColors mapColors { get; set; } = new();

    private Playfield playfield;
    private List<ColorableSkinDrawable> colorableDrawables;

    public Colour4 Primary => mapColors.Primary;

    public Colour4 Secondary => mapColors.Secondary;

    public Colour4 Middle => mapColors.Middle;

    public event Action<Colour4> ColorChanged;

    public ColorManager(Playfield playfield)
    {
        this.playfield = playfield;
        colorableDrawables = new();
    }

    public void Register(ColorableSkinDrawable skinDrawable) => colorableDrawables.Add(skinDrawable);
    public void Unregister(ColorableSkinDrawable skinDrawable) => colorableDrawables.Remove(skinDrawable);

    public void SetColor(int lane, Colour4 color)
    {
        // Not implemented yet
        ColorChanged?.Invoke(color);
    }

    public void FadeColor(int lane, Colour4 color, double duration = 0, Easing easing = Easing.None)
    {
        // Not implemented yet
        ColorChanged?.Invoke(color);
    }

    public bool HasColorFor(int lane, int keyCount, out Colour4 colour) => mapColors.HasColorFor(lane, keyCount, out colour);

    public Colour4 GetColor(int index, Colour4 fallback) => mapColors.GetColor(index, fallback);
}