using System.Collections.Generic;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Screens.Layout.Blueprints.Selection;

public partial class ComponentSelectionHandler : SelectionHandler<GameplayHUDComponent>
{
    [Resolved]
    private LayoutEditor editor { get; set; }

    public override void Delete(IEnumerable<GameplayHUDComponent> objects) => objects.ForEach(editor.RemoveComponent);
}
