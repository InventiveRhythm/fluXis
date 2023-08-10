using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintLongNoteBody : BlueprintNotePiece
{
    public BlueprintLongNoteBody()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.Centre;
        Alpha = 0.5f;
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Selection
        };
    }
}
