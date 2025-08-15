using System.IO;
using fluXis.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.UserInterface.Files.Entry;

public partial class FileEntry : GenericEntry
{
    protected override IconUsage Icon => PathUtils.GetIconForType(type);
    public override string Text => file.Name;
    public override bool IsInfoLoaded => isLoaded;
    protected override string SubText => info;
    protected override Colour4 Color => PathUtils.GetColorForType(type);

    private PathUtils.FileType type => PathUtils.GetTypeForExtension(file.Extension);

    private FileInfo file { get; }
    private FileSelect selector { get; }

    private bool isLoaded = false;
    private string info = default_subtext;

    private const string default_subtext = "???KB";

    public FileEntry(FileInfo file, FileSelect selector)
    {
        this.file = file;
        this.selector = selector;
    }

    public override string GetInfo()
    {
        if (isLoaded) return info;
        info = getSize();
        isLoaded = true;
        return info;
    }

    protected override void LoadComplete()
    {
        selector.FileChanged += fileChanged;
        fileChanged(selector.CurrentFile);
        base.LoadComplete();
    }

    protected override bool OnClick(ClickEvent e)
    {
        selector.SelectFile(file);
        return base.OnClick(e);
    }

    private void fileChanged(FileInfo info) => SetSelected(info?.FullName == file.FullName);

    private string getSize()
    {
        var size = file.Length;
        var suffix = new[] { "B", "KB", "MB", "GB", "TB" };
        var i = 0;

        while (size > 1024)
        {
            size /= 1024;
            i++;
        }

        return size + suffix[i];
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        selector.FileChanged -= fileChanged;
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        selector.OnSelect();
        return base.OnDoubleClick(e);
    }
}
