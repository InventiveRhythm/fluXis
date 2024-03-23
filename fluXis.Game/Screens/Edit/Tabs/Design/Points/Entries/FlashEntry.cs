using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class FlashEntry : PointListEntry
{
    protected override string Text => "Flash";
    protected override Colour4 Color => Colour4.FromHex("#FF00FF");

    private FlashEvent flash => Object as FlashEvent;

    public FlashEntry(FlashEvent obj)
        : base(obj)
    {
    }

    protected override Drawable[] CreateValueContent()
    {
        return new Drawable[]
        {
            new Circle
            {
                Size = new Vector2(20),
                Colour = flash.StartColor,
                Margin = new MarginPadding { Right = 4 }
            },
            new Circle
            {
                Size = new Vector2(20),
                Colour = flash.EndColor,
                Margin = new MarginPadding { Right = 10 }
            },
            new FluXisSpriteText
            {
                Text = $"{flash.Duration}ms",
                Colour = Color
            }
        };
    }
}
