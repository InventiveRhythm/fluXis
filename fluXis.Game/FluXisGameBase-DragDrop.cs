using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Game;

public partial class FluXisGameBase
{
    private List<IDragDropHandler> dragDropHandlers { get; } = new();

    public void AddDragDropHandler(IDragDropHandler handler) => dragDropHandlers.Add(handler);
    public void RemoveDragDropHandler(IDragDropHandler handler) => dragDropHandlers.Remove(handler);

    public void HandleDragDrop(string[] files) => files.ForEach(HandleDragDrop);

    public void HandleDragDrop(string file)
    {
        var handler = dragDropHandlers.FirstOrDefault(h => h.AllowedExtensions.Contains(Path.GetExtension(file)) && h.OnDragDrop(file));

        if (handler == null)
            importManager.Import(file);
    }
}
