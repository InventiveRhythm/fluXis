using System.IO;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Files.Sidebar;

public partial class SidebarEntry : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    private IconUsage icon { get; }
    private string text { get; }
    private DirectoryInfo directory { get; }
    private FileSelect fileSelect { get; }

    private Container content { get; set; }
    private HoverLayer hover { get; set; }
    private FlashLayer flash { get; set; }

    public SidebarEntry(IconUsage icon, string text, DirectoryInfo directory, FileSelect fileSelect)
    {
        this.icon = icon;
        this.text = text;
        this.directory = directory;
        this.fileSelect = fileSelect;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 64;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Icon = icon,
                    Size = new Vector2(28),
                    Margin = new MarginPadding { Left = 18 }
                },
                new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = text,
                    FontSize = 24,
                    Margin = new MarginPadding { Left = 64 }
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.Show();
        return false; // pass through to sidebar
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        flash.Show();
        fileSelect.SelectDirectory(directory);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }
}
