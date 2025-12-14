using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintNotePiece : Container
{
    public BlueprintNotePiece()
    {
        Height = 10;
        CornerRadius = 5;
        Masking = true;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.Centre;
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Theme.Selection
        };
    }

    public void MouseDown() => this.ResizeWidthTo(0.75f, 150, Easing.OutBack);
    public void MouseUp() => this.ResizeWidthTo(0.5f, 300, Easing.OutQuint);
}
