using System.IO;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.FileSelect;

public partial class DirectorySelectDirectory : DirectorySelectorDirectory
{
    public DirectorySelectDirectory(DirectoryInfo directory, string displayName = null)
        : base(directory, displayName)
    {
    }

    protected override IconUsage? Icon { get; } = FontAwesome.Regular.Folder;
}
