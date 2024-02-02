using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace fluXis.Game.Map.Drawables;

public partial class MapBackground : Sprite
{
    [CanBeNull]
    private RealmMap map;

    [Resolved]
    private SkinManager skinManager { get; set; }

    [CanBeNull]
    public RealmMap Map
    {
        get => map;
        set
        {
            map = value;

            if (IsLoaded)
                setTexture();
        }
    }

    private bool cropped { get; }

    public MapBackground([CanBeNull] RealmMap map, bool cropped = false)
    {
        this.map = map;
        this.cropped = cropped;

        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load() => setTexture();

    public Colour4 GetColour()
    {
        var stream = map?.GetBackgroundStream();

        if (stream == null)
            return Colour4.Transparent;

        var image = Image.Load<Rgba32>(stream);
        image.Mutate(x => x.Quantize(new WuQuantizer(new QuantizerOptions { MaxColors = 10 })));

        var dict = new Dictionary<Rgba32, int>();

        for (var x = 0; x < image.Width; x++)
        {
            for (var y = 0; y < image.Height; y++)
            {
                var pixel = image[x, y];

                if (pixel.A == 0)
                    continue;

                if (!dict.TryAdd(pixel, 1))
                    dict[pixel]++;
            }
        }

        var orderedByLight = dict.Select(x => x.Key).Select(x => new Colour4(x.R, x.G, x.B, 255)).OrderBy(x => x.ToHSL().Z).ToList();
        return orderedByLight.ElementAt(orderedByLight.Count / 2);
    }

    private void setTexture()
    {
        var custom = cropped ? Map?.GetPanelBackground() : Map?.GetBackground();
        Texture = custom ?? skinManager.GetDefaultBackground();
    }
}
