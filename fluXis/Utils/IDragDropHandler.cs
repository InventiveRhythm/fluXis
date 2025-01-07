namespace fluXis.Utils;

public interface IDragDropHandler
{
    public string[] AllowedExtensions { get; }
    public bool OnDragDrop(string file);
}
