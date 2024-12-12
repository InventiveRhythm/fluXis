using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Models.Scores;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.User.Tabs.Scores;

public partial class ProfileScoresSection : FillFlowContainer
{
    public List<APIScore> Maps
    {
        set
        {
            var empty = value.Count <= 0;

            noScores.Alpha = empty ? .8f : 0;
            flow.Alpha = empty ? 0 : 1;

            flow.ChildrenEnumerable = value.Take(8).Select(map => new ProfileScore(map));
        }
    }

    private FluXisSpriteText noScores { get; }
    private FillFlowContainer flow { get; }

    public ProfileScoresSection(string title)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 8 },
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = title,
                        WebFontSize = 20
                    },
                    noScores = new FluXisSpriteText
                    {
                        Text = "Nothing here yet...",
                        WebFontSize = 12
                    }
                }
            },
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(12)
            }
        };
    }
}
