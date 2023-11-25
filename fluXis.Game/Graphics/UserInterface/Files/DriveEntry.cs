using System;
using System.IO;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Files;

public partial class DriveEntry : Container
{
    [Resolved]
    private UISamples samples { get; set; }

    private DriveInfo info { get; }
    private FileSelect selector { get; }

    private float usage => (float)info.AvailableFreeSpace / info.TotalSize;
    private Colour4 color { get; }

    private Box hover { get; set; }

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
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color,
                Alpha = 0
            },
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
                            new SpriteIcon
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
                                Text = $"{info.Name}{info.VolumeLabel}",
                                Margin = new MarginPadding { Left = 35 }
                            },
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Alpha = .8f,
                                Text = $"{info.AvailableFreeSpace / 1024 / 1024 / 1024} GB / {info.TotalSize / 1024 / 1024 / 1024} GB",
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
        hover.FadeTo(.2f, 50);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }
}
