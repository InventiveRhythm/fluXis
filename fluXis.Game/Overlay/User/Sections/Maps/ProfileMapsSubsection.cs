using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Drawables.Card;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.User.Sections.Maps;

public partial class ProfileMapsSubsection : FillFlowContainer
{
    private string title { get; }
    private List<APIMapSet> maps { get; }

    public ProfileMapsSubsection(string title, List<APIMapSet> maps)
    {
        this.title = title;
        this.maps = maps;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(10);

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 10 },
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = title,
                        WebFontSize = 20
                    },
                    new FluXisSpriteText
                    {
                        Text = $"This user has no {title.ToLower()} maps.",
                        Alpha = maps.Any() ? 0 : .6f,
                        WebFontSize = 16
                    }
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(20),
                Alpha = maps.Any() ? 1 : 0,
                ChildrenEnumerable = maps.Select(map => new MapCard(map) { CardWidth = 448 })
            }
        };
    }
}
