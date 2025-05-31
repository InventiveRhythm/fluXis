using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Map.Drawables.Card;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.User.Tabs.Maps;

public partial class ProfileMapsSection : FillFlowContainer
{
    public List<APIMapSet> Maps
    {
        set
        {
            var empty = value.Count <= 0;

            noMaps.Alpha = empty ? .8f : 0;
            flow.Alpha = empty ? 0 : 1;

            flow.ChildrenEnumerable = value.Select(map => new MapCard(map) { CardWidth = 454 });
        }
    }

    private FluXisSpriteText noMaps { get; }
    private FillFlowContainer flow { get; }

    public ProfileMapsSection(string title)
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
                    noMaps = new FluXisSpriteText
                    {
                        Text = $"This user has no {title.ToLower()} maps.",
                        WebFontSize = 12
                    }
                }
            },
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(16)
            }
        };
    }
}
