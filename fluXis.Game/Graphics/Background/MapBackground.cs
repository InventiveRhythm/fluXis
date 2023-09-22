using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace fluXis.Game.Graphics.Background;

public partial class MapBackground : Sprite
{
    [Resolved(CanBeNull = true)]
    private ImportManager importManager { get; set; }

    public RealmMap Map { get; set; }
    public bool Cropped { get; set; }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        if (Map == null)
            Texture = skinManager.GetDefaultBackground();
        else
            Texture = (Cropped ? Map.GetPanelBackground() : Map.GetBackground()) ?? skinManager.GetDefaultBackground();
    }

    public Colour4 GetColour()
    {
        var stream = Map?.GetBackgroundStream();

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

                if (dict.ContainsKey(pixel))
                    dict[pixel]++;
                else
                    dict.Add(pixel, 1);
            }
        }

        var orderedByLight = dict.Select(x => x.Key).Select(x => new Colour4(x.R, x.G, x.B, 255)).OrderBy(x => x.ToHSL().Z).ToList();
        return orderedByLight.ElementAt(orderedByLight.Count / 2);
    }
}
