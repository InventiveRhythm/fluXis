using System.IO;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.UserInterface.FileSelect;

public partial class DirectorySelectParentDirectory : DirectorySelectDirectory
{
    protected override IconUsage? Icon { get; } = FontAwesome.Solid.ChevronUp;

    public DirectorySelectParentDirectory(DirectoryInfo directory)
        : base(directory, "Up one level")
    {
    }
}
