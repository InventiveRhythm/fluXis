using System;
using fluXis.Graphics.Containers;
using fluXis.Online;
using fluXis.Online.Drawables.Images;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Tests.Graphics;

public partial class TestDrawableCountry : FluXisTestScene
{
    public TestDrawableCountry()
    {
        var countries = Enum.GetValues<CountryCode>();

        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Full,
            Spacing = new Vector2(10)
        };

        foreach (var countryCode in countries)
        {
            flow.Add(new DrawableCountry(countryCode)
            {
                Width = 80
            });
        }

        flow.Add(new DrawableCountry(CountryUtils.GetCountryCode("nonexisitent"))
        {
            Width = 80
        });

        Add(new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = flow
        });
    }
}
