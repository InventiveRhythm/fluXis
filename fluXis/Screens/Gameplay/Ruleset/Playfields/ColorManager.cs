using System;
using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class ColorManager : Component, ICustomColorProvider
{
    [Resolved(CanBeNull = true)]
    private ICustomColorProvider mapColors { get; set; }

    private Playfield playfield;
    private DependencyContainer dependencies;
    private List<ColorableSkinDrawable> colorableDrawables;

    public Colour4 Primary { get; private set; } = Theme.Primary;
    public Colour4 Secondary { get; private set; } = Theme.Secondary;
    public Colour4 Middle { get; private set; } = Colour4.White;

    public event Action<Colour4> ColorChanged;

    public ColorManager(Playfield playfield, DependencyContainer dependencies)
    {
        this.playfield = playfield;
        this.dependencies = dependencies;
        colorableDrawables = new();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Primary = mapColors?.Primary ?? Primary;
        Secondary = mapColors?.Primary ?? Secondary;
        Middle = mapColors?.Middle ?? Middle;
    }

    public void Register(ColorableSkinDrawable skinDrawable) => colorableDrawables.Add(skinDrawable);
    public void Unregister(ColorableSkinDrawable skinDrawable) => colorableDrawables.Remove(skinDrawable);

    private void updateColor(MapColor index, Colour4 color)
    {
        switch (index)
        {
            case MapColor.Primary:
                Primary = color;
                break;

            case MapColor.Secondary:
                Secondary = color;
                break;

            case MapColor.Middle:
                Middle = color;
                break;
        }
    }

    private void updateCache()
    {
        var cachedMapColors = (MapColors)mapColors;
        foreach (var cachedDrawable in cachedMapColors.CachedDrawables)
        {
            if (!colorableDrawables.Contains(cachedDrawable))
            {
                cachedDrawable.ResolveProviderFrom(dependencies);
                colorableDrawables.AddRange(cachedMapColors.CachedDrawables);
            }
        }
    }

    public void SetColor(MapColor index, Colour4 color)
    {
        updateCache();

        colorableDrawables.ForEach(drawable =>
        {
            if (drawable.Index == index)
            {
                drawable.SetColor(color);

                switch (index)
                {
                    case MapColor.Primary:
                        drawable.SetColorGradient(color, Secondary);
                        break;

                    case MapColor.Secondary:
                        drawable.SetColorGradient(Primary, color);
                        break;
                }
                
                updateColor(drawable.Index, color);
            }
        });
        ColorChanged?.Invoke(color);
    }

    public void FadeColor(MapColor index, Colour4 color, double duration = 0, Easing easing = Easing.None)
    {
        updateCache();

        colorableDrawables.ForEach(drawable =>
        {
            if (drawable.Index == index || drawable.Index == MapColor.Gradient)
            {
                drawable.FadeColor(color, duration, easing);

                switch (index)
                {
                    case MapColor.Primary:
                        drawable.FadeColorGradient(color, Secondary, duration, easing);
                        break;

                    case MapColor.Secondary:
                        drawable.FadeColorGradient(Primary, color, duration, easing);
                        break;
                }
                
                updateColor(drawable.Index, color);
            }
        });
        ColorChanged?.Invoke(color);
    }

    public bool HasColorFor(int lane, int keyCount, out Colour4 colour)
    {
        var index = Theme.GetLaneColorIndex(lane, keyCount);
        colour = GetColor(index, Colour4.Transparent);
        return colour != Colour4.Transparent;
    }

    public Colour4 GetColor(int index, Colour4 fallback)
    {
        var colors = new[]
        {
            Colour4.Transparent,
            Primary,
            Secondary,
            Middle
        };

        if (index < 0 || index >= colors.Length)
            return fallback;

        var col = colors[index];

        if (col == Colour4.Transparent)
            return fallback;

        return col;
    }
}