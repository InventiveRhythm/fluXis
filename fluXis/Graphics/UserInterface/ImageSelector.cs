using System;
using System.IO;
using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Panel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Graphics.UserInterface;

#nullable enable

public partial class ImageSelector : CompositeDrawable
{
    [Resolved]
    private UISamples samples { get; set; } = null!;

    [Resolved]
    private PanelContainer panels { get; set; } = null!;

    private SpriteStack<Drawable> stack { get; }

    public Action<ImageSelector, FileInfo>? OnFileSelected { get; init; } = null!;

    public ImageSelector()
    {
        CornerRadius = 8;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            stack = new SpriteStack<Drawable>
            {
                RelativeSizeAxes = Axes.Both,
                AutoFill = true
            },
            new ImageEditHover()
        };
    }

    public void AddSprite(Drawable sprite) => stack.Add(sprite);

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        panels.Add(new FileSelect
        {
            AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
            OnFileSelected = f => OnFileSelected?.Invoke(this, f)
        });
        return true;
    }

    public partial class ImageEditHover : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; } = null!;

        public ImageEditHover()
        {
            RelativeSizeAxes = Axes.Both;
            AlwaysPresent = true;
            Alpha = 0;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2,
                    Alpha = 0.75f
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(8),
                    Children = new Drawable[]
                    {
                        new FluXisSpriteIcon
                        {
                            Size = new Vector2(16),
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Icon = FontAwesome6.Solid.Pencil
                        },
                        new ForcedHeightText
                        {
                            Height = 28,
                            WebFontSize = 12,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "click to change"
                        }
                    }
                }
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            this.FadeIn(50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.FadeOut(200);
        }
    }
}
