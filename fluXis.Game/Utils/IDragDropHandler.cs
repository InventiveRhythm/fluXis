namespace fluXis.Game.Utils;

public interface IDragDropHandler
{
    public string[] AllowedExtensions { get; }
    public bool OnDragDrop(string file);
}
