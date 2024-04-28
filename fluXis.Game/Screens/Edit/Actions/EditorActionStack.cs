using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using JetBrains.Annotations;

namespace fluXis.Game.Screens.Edit.Actions;

public class EditorActionStack
{
    private List<EditorAction> stack { get; } = new();
    private int index { get; set; } = -1;

    public bool CanUndo => index > -1;
    public bool CanRedo => index < stack.Count - 1;

    [CanBeNull]
    public NotificationManager NotificationManager { get; init; }

    private readonly EditorMap editorMap;

    public EditorActionStack(EditorMap editorMap)
    {
        this.editorMap = editorMap;
    }

    public void Add(EditorAction action)
    {
        if (index < 0)
            stack.Clear();
        else if (index < stack.Count - 1)
            stack.RemoveRange(index, stack.Count - index);

        action.Run(editorMap);
        stack.Add(action);
        index = stack.Count - 1;
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            NotificationManager?.SendSmallText("Nothing to undo.", FontAwesome6.Solid.XMark);
            return;
        }

        var action = stack[index];
        action.Undo(editorMap);

        var desc = action.Description;

        if (!string.IsNullOrWhiteSpace(desc))
            NotificationManager?.SendSmallText($"Undo '{desc}'.", FontAwesome6.Solid.RotateLeft);

        index--;
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            NotificationManager?.SendSmallText("Nothing to redo.", FontAwesome6.Solid.XMark);
            return;
        }

        index++;

        var action = stack[index];
        action.Run(editorMap);

        var desc = action.Description;

        if (!string.IsNullOrWhiteSpace(desc))
            NotificationManager?.SendSmallText($"Redo '{desc}'.", FontAwesome6.Solid.RotateRight);
    }

    public override string ToString()
    {
        return $"Index: {index}, Items: {stack.Count}, CanUndo: {CanUndo}, CanRedo: {CanRedo}";
    }
}

