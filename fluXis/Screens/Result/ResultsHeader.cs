using System.Collections.Generic;
using fluXis.Online.API.Models.Users;
using fluXis.Screens.Result.Header;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Result;

public partial class ResultsHeader : CompositeDrawable
{
    [Resolved]
    private List<APIUser> players { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(16);

        InternalChildren = new Drawable[]
        {
            new ResultsMap(),
        };

        int x = -300 * (players.Count - 1);
        int i = 0;

        foreach (var player in players)
        {
            AddInternal(new ResultsPlayer(player, i)
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                X = x,
            });
            x += 300;
            i++;
        }
    }

    //TODO: update this whenever we switch scoreInfo in the result screen
}
