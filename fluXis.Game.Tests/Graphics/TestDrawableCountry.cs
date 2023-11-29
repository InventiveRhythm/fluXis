using System;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Drawables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Tests.Graphics;

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

        flow.Add(new DrawableCountry(APIUserShort.GetCountryCode("nonexisitent"))
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
