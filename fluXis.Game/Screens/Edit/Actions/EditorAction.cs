namespace fluXis.Game.Screens.Edit.Actions;

public abstract class EditorAction
{
    public abstract string Description { get; }
    public abstract void Run();
    public abstract void Undo();
}

