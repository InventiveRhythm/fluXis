using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace fluXis.Game.Utils;

public static class ImageUtils
{
    public static Colour4 GetAverageColour(Stream stream)
    {
        if (stream == null)
            return Colour4.Transparent;

        try
        {
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
        catch (Exception e)
        {
            Logger.Error(e, "Failed to get average colour from image");
            return Colour4.Transparent;
        }
    }
}
