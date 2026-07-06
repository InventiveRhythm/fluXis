using fluXis.Graphics.Sprites.Outline;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Graphics;

namespace fluXis.Screens.Layout.Blueprints.Selection;

public partial class ComponentSelectionBlueprint : SelectionBlueprint<GameplayHUDComponent>
{
    public ComponentSelectionBlueprint(GameplayHUDComponent component)
        : base(component)
    {
        Anchor = Origin = Anchor.TopLeft;

        Child = new OutlinedSquare
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Theme.Highlight,
            BorderThickness = 4,
            CornerRadius = 4
        };
    }

    protected override void Update()
    {
        base.Update();

        var quad = Parent!.ToLocalSpace(Drawable.ScreenSpaceDrawQuad);

        Size = quad.Size;
        Position = quad.TopLeft;
    }
}
