using System;
using System.IO;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface.Files;

public partial class DriveEntry : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    private DriveInfo info { get; }
    private FileSelect selector { get; }

    private long used => info.TotalSize - info.AvailableFreeSpace;
    private float usage => (float)used / info.TotalSize;
    private Colour4 color { get; }

    private HoverLayer hover { get; set; }

    public DriveEntry(DriveInfo info, FileSelect selector)
    {
        this.info = info;
        this.selector = selector;

        var h = 240 - 240 * usage;
        h = Math.Clamp(h, 0, 240);

        color = Colour4.FromHSL(h / 360f, .8f, .8f);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 80;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            hover = new HoverLayer { Colour = color },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding { Horizontal = 20 },
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(20),
                                Margin = new MarginPadding(5),
                                Icon = PathUtils.GetIconForType(PathUtils.FileType.Drive)
                            },
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = OperatingSystem.IsLinux() ? info.VolumeLabel : $"{info.Name}{info.VolumeLabel}",
                                Margin = new MarginPadding { Left = 35 }
                            },
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Alpha = .8f,
                                Text = $"{toOptimalSizeString(used)} / {toOptimalSizeString(info.TotalSize)}",
                            }
                        }
                    },
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 10,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = color,
                                Alpha = .2f
                            },
                            new Circle
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = usage,
                                Colour = color
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        selector.SelectDirectory(info.RootDirectory);
        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    private static string toOptimalSizeString(long sizeB)
    {
        string[] suffixes = { "B", "KiB", "MiB", "GiB", "TiB" };

        long maxB = 1024;
        long size = sizeB;

        for (int i = 0; i < suffixes.Length - 1; i++)
        {
            if (sizeB < maxB)
            {
                return $"{size} {suffixes[i]}";
            }

            maxB *= 1024;
            size /= 1024;
        }

        return $"{size} {suffixes.Last()}";
    }
}
