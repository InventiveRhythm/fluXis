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
    }

    protected override void Update()
    {
        base.Update();
        Size = Drawable.ScreenSpaceDrawQuad.Size;
        Position = Parent!.ToLocalSpace(Drawable.ScreenSpaceDrawQuad).TopLeft;
    }
}
