using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Utils;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Logging;

namespace fluXis;

public partial class FluXisGameBase
{
    private List<IDragDropHandler> dragDropHandlers { get; } = new();

    public void AddDragDropHandler(IDragDropHandler handler) => dragDropHandlers.Add(handler);
    public void RemoveDragDropHandler(IDragDropHandler handler) => dragDropHandlers.Remove(handler);

    public void HandleDragDrop(string[] files) => files.ForEach(HandleDragDrop);

    public void HandleDragDrop(string file)
    {
        try
        {
            var handler = dragDropHandlers.FirstOrDefault(h => h.AllowedExtensions.Contains(Path.GetExtension(file)) && h.OnDragDrop(file));

            if (handler == null)
                importManager.Import(file);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to handle drag drop");
        }
    }
}
