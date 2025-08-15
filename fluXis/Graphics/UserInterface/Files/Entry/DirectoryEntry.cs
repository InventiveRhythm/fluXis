using System.IO;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.UserInterface.Files.Entry;

public partial class DirectoryEntry : GenericEntry
{
    protected override IconUsage Icon => PathUtils.GetIconForType(PathUtils.FileType.Folder);
    public override string Text => directory.Name;
    public override bool IsInfoLoaded => isLoaded;
    protected override string SubText => info;
    protected override Colour4 Color => PathUtils.GetColorForType(PathUtils.FileType.Folder);

    private DirectoryInfo directory { get; }
    private FileSelect select { get; }

    private bool isLoaded = false;
    private string info = default_subtext;

    private const string default_subtext = "???KB";

    public DirectoryEntry(DirectoryInfo directory, FileSelect selector)
    {
        this.directory = directory;
        select = selector;
    }

    public override string GetInfo()
    {
        if (isLoaded) return info;
        info = tryGetFiles();
        isLoaded = true;
        return info;
    }

    protected override bool OnClick(ClickEvent e)
    {
        select.SelectDirectory(directory);
        return base.OnClick(e);
    }

    private string tryGetFiles()
    {
        try
        {
            var files = directory.GetFiles();
            var folders = directory.GetDirectories();

            return files.Length + folders.Length + " Items";
        }
        catch
        {
            return "??? Items";
        }
    }
}
