using System.Collections.Generic;
using fluXis.Game.Screens.Edit.Actions;

namespace fluXis.Game.Screens.Edit;

public class EditorActionStack
{
    private List<EditorAction> stack { get; } = new();
    private int index { get; set; } = -1;

    public bool CanUndo => index > -1;
    public bool CanRedo => index < stack.Count - 1;

    public void Add(EditorAction action)
    {
        if (index < stack.Count - 1)
            stack.RemoveRange(index, stack.Count - index);

        action.Run();
        stack.Add(action);
        index = stack.Count - 1;
    }

    public void Undo()
    {
        if (!CanUndo)
            return;

        stack[index].Undo();
        index--;
    }

    public void Redo()
    {
        if (!CanRedo)
            return;

        index++;
        stack[index].Run();
    }
}

