using System.Collections.Generic;
using fluXis.Screens.Edit;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Map.Structures.Bases;

public interface IWithContext
{
    IEnumerable<MenuItem> CreateContextItems(EditorMap map, EditorSnapProvider snaps);
}
