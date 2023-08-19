using System.IO;
using fluXis.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.UserInterface.FileSelect;

public partial class FileSelect : FileSelector
{
    public new MarginPadding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    public FileSelect(string initialPath = null, string[] validFileExtensions = null)
        : base(initialPath, validFileExtensions)
    {
        Padding = new MarginPadding(25);
    }

    protected override ScrollContainer<Drawable> CreateScrollContainer() => new FluXisScrollContainer { ScrollbarAnchor = Anchor.TopRight };
    protected override DirectorySelectorBreadcrumbDisplay CreateBreadcrumb() => new DirectorySelectBreadCrumb();
    protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null) => new DirectorySelectDirectory(directory, displayName);
    protected override DirectorySelectorDirectory CreateParentDirectoryItem(DirectoryInfo directory) => new DirectorySelectParentDirectory(directory);
    protected override DirectoryListingFile CreateFileItem(FileInfo file) => new FileSelectFile(file);

    protected override Drawable CreateHiddenToggleButton() => new DirectorySelectHiddenToggle();

    protected partial class FileSelectFile : DirectoryListingFile
    {
        public FileSelectFile(FileInfo file)
            : base(file)
        {
        }

        protected override IconUsage? Icon
        {
            get
            {
                switch (File.Extension)
                {
                    case ".mp3":
                    case ".wav":
                    case ".ogg":
                        return FontAwesome.Solid.Music;

                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        return FontAwesome.Solid.Image;

                    case ".mp4":
                    case ".mov":
                    case ".avi":
                    case ".flv":
                    case ".mpg":
                    case ".wmv":
                    case ".m4v":
                        return FontAwesome.Solid.Video;

                    case ".fms":
                    case ".osu":
                    case ".qp":
                        return FontAwesome.Solid.Map;

                    case ".fsk":
                        return FontAwesome.Solid.PaintBrush;

                    default:
                        return FontAwesome.Solid.File;
                }
            }
        }
    }
}

