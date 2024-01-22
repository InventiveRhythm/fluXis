using System;
using System.IO;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Files.Sidebar;

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
                        new(FontAwesome6.Solid.HardDrive, "This PC", null, select),
                        new(FontAwesome6.Solid.Display, "Desktop", getFor(Environment.SpecialFolder.Desktop), select),
                        new(FontAwesome6.Solid.User, Environment.UserName, getFor(Environment.SpecialFolder.UserProfile), select),
                        new(FontAwesome6.Solid.Download, "Downloads", getFor(Environment.SpecialFolder.UserProfile).CreateSubdirectory("Downloads"), select),
                        new(FontAwesome6.Solid.Images, "Pictures", getFor(Environment.SpecialFolder.MyPictures), select),
                        new(FontAwesome6.Solid.Music, "Music", getFor(Environment.SpecialFolder.MyMusic), select)
                    }
                }
            }
        };

        if (!string.IsNullOrEmpty(mapDirectory))
            entries.Add(new SidebarEntry(FontAwesome6.Solid.Cube, "Map Folder", new DirectoryInfo(mapDirectory), select));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Expanded.BindValueChanged(e => this.ResizeWidthTo(e.NewValue ? 250 : 84, 500, Easing.OutQuint), true);
    }

    private static DirectoryInfo getFor(Environment.SpecialFolder folder)
        => new(Environment.GetFolderPath(folder));
}
