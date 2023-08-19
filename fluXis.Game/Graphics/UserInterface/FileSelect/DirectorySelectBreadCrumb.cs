using System.IO;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.FileSelect;

public partial class DirectorySelectBreadCrumb : DirectorySelectorBreadcrumbDisplay
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Margin = new MarginPadding { Bottom = 10 };

        if (InternalChild is FillFlowContainer flow)
            flow.Padding = new MarginPadding(10);

        AddInternal(new Container
        {
            Masking = true,
            CornerRadius = 10,
            RelativeSizeAxes = Axes.Both,
            Depth = 1,
            Child = new Box
            {
                Colour = FluXisColors.Background1,
                RelativeSizeAxes = Axes.Both
            }
        });
    }

    protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, string displayName = null) => new BreadCrumbDirectory(directory, displayName);
    protected override DirectorySelectorDirectory CreateRootDirectoryItem() => new DirectorySelectComputer();

    private partial class DirectorySelectComputer : DirectorySelectDirectory
    {
        protected override IconUsage? Icon => FontAwesome.Regular.Hdd;

        public DirectorySelectComputer()
            : base(null, "Computer")
        {
        }
    }

    private partial class BreadCrumbDirectory : DirectorySelectorDirectory
    {
        protected override IconUsage? Icon => FontAwesome.Regular.Folder;

        public BreadCrumbDirectory(DirectoryInfo directory, string displayName = null)
            : base(directory, displayName)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Flow.Add(new SpriteIcon
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Icon = FontAwesome.Solid.ChevronRight,
                Size = new Vector2(FONT_SIZE / 2),
                Margin = new MarginPadding { Left = 5 }
            });
        }
    }
}
