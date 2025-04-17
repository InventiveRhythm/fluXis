using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Layout.Blueprints.Selection;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Input.Events;

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

    protected override SelectionHandler<GameplayHUDComponent> CreateSelectionHandler() => new ComponentSelectionHandler();

    protected override void MoveSelection(DragEvent e)
    {
        base.MoveSelection(e);

        SelectedObjects.ForEach(c =>
        {
            c.Settings.Position -= e.ScreenSpaceLastMousePosition - e.ScreenSpaceMousePosition;
            c.Settings.ApplyTo(c);
        });
    }
}
