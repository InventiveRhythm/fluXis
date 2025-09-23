using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields;

public partial class ColorManager : Component, ICustomColorProvider
{
    private List<(ColorableSkinDrawable draw, MapColor idx)> drawables { get; } = new();

    private Colour4 primary = Theme.GetLaneColor(1).Lighten(.2f);
    private Colour4 secondary = Theme.GetLaneColor(2).Lighten(.2f);
    private Colour4 middle = Theme.GetLaneColor(3).Lighten(.2f);

    public Colour4 Primary
    {
        get => primary;
        private set
        {
            if (primary == value) return;

            primary = value;
            SetColor(MapColor.Primary, value);
        }
    }

    public Colour4 Secondary
    {
        get => secondary;
        private set
        {
            if (secondary == value) return;

            secondary = value;
            SetColor(MapColor.Secondary, value);
        }
    }

    public Colour4 Middle
    {
        get => middle;
        private set
        {
            if (middle == value) return;

            middle = value;
            SetColor(MapColor.Middle, value);
        }
    }

    [BackgroundDependencyLoader(true)]
    private void load([CanBeNull] ICustomColorProvider mapColors)
    {
        Primary = getDefaultColor(mapColors?.Primary, Primary);
        Secondary = getDefaultColor(mapColors?.Secondary, Secondary);
        Middle = getDefaultColor(mapColors?.Middle, Middle);
    }

    private static Colour4 getDefaultColor(Colour4? colour, Colour4 fallback)
    {
        if (colour is null || colour == Colour4.Transparent)
            return fallback;

        return colour.Value;
    }

    public void Register(ColorableSkinDrawable draw, MapColor index) => drawables.Add((draw, index));
    public void Unregister(ColorableSkinDrawable draw, MapColor index) => drawables.RemoveAll(x => x.draw == draw && x.idx == index);

    public void SetColor(MapColor index, Colour4 color)
    {
        var match = drawables.Where(x => x.idx == index).ToList();
        match.ForEach(x => x.draw.UpdateColor(index, color));
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
