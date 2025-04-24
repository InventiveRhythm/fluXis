namespace fluXis.Integration;

#nullable enable

public interface IWorkshopItem
{
    string Title { get; }
    string? Preview { get; }
    string Folder { get; }

    public string? ToString() => $"{Title} ({Folder})";
}

public class BasicWorkshopItem : IWorkshopItem
{
    public string Title { get; }
    public string? Preview { get; }
    public string Folder { get; }

    public BasicWorkshopItem(string title, string? preview, string folder)
    {
        Title = title;
        Preview = preview;
        Folder = folder;
    }
}
