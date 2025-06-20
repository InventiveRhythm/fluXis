using System;
using System.IO;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Graphics.UserInterface.Files.Sidebar;

public partial class FileSelectSidebar : ExpandingContainer
{
    protected override double HoverDelay => 500;

    private FileSelect select { get; }
    private string mapDirectory { get; }

    public FileSelectSidebar(FileSelect select, string mapDirectory)
    {
        this.select = select;
        this.mapDirectory = mapDirectory;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 84;
        RelativeSizeAxes = Axes.Y;
        Masking = true;

        FillFlowContainer<SidebarEntry> entries;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                ScrollbarVisible = false,
                Child = entries = new FillFlowContainer<SidebarEntry>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10),
                    Children = new SidebarEntry[]
                    {
                        new(FontAwesome6.Solid.HardDrive, "This PC", null, select)
                    }
                }
            }
        };

        string folder;

        if (tryGetFolderPath(Environment.SpecialFolder.Desktop, out folder))
        {
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Display, "Desktop", new DirectoryInfo(folder), select));
        }

        if (tryGetFolderPath(Environment.SpecialFolder.UserProfile, out folder))
        {
            entries.Add(new SidebarEntry(FontAwesome6.Solid.User, Environment.UserName, new DirectoryInfo(folder), select));
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Download, "Downloads", new DirectoryInfo(folder).CreateSubdirectory("Downloads"), select));
        }

        if (tryGetFolderPath(Environment.SpecialFolder.MyPictures, out folder))
        {
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Images, "Pictures", new DirectoryInfo(folder), select));
        }

        if (tryGetFolderPath(Environment.SpecialFolder.MyMusic, out folder))
        {
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Music, "Music", new DirectoryInfo(folder), select));
        }

        if (!string.IsNullOrEmpty(mapDirectory))
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Cube, "Map Folder", new DirectoryInfo(mapDirectory), select));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Expanded.BindValueChanged(e => this.ResizeWidthTo(e.NewValue ? 250 : 84, 500, Easing.OutQuint), true);
    }

    private static bool tryGetFolderPath(Environment.SpecialFolder folder, out string path)
    {
        path = Environment.GetFolderPath(folder);

        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return true;
    }
}
