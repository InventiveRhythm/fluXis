using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Layout.Blueprints.Selection;
using osu.Framework.Allocation;

namespace fluXis.Screens.Layout.Blueprints;

public partial class LayoutBlueprintContainer : BlueprintContainer<GameplayHUDComponent>
{
    [Resolved]
    private LayoutEditor editor { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        editor.ComponentAdded += AddBlueprint;
    }

    protected override SelectionBlueprint<GameplayHUDComponent> CreateBlueprint(GameplayHUDComponent obj)
    {
        var blueprint = new ComponentSelectionBlueprint(obj);
        blueprint.Drawable = obj;
        return blueprint;
    }
}
