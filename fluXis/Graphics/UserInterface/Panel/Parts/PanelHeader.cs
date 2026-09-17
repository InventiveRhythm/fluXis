using System;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel.Parts;

public partial class PanelHeader : GridContainer
{
    public const float HEIGHT = 28;

    public PanelHeader(IconUsage icon, LocalisableString title, Action act)
    {
        RelativeSizeAxes = Axes.X;
        Height = HEIGHT;
        ColumnDimensions =
        [
            new Dimension(GridSizeMode.Absolute, HEIGHT),
            new Dimension(GridSizeMode.Absolute, 12),
            new Dimension(),
            new Dimension(GridSizeMode.Absolute, 12),
            new Dimension(GridSizeMode.Absolute, HEIGHT)
        ];
        Content = new[]
        {
            new[]
            {
                new FluXisSpriteIcon
                {
                    Icon = icon,
                    Size = new Vector2(20),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                Empty(),
                new TruncatingText
                {
                    Text = title,
                    WebFontSize = 20,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.X
                },
                Empty(),
                new CloseButton { Action = act }
            }
        };
    }

    private partial class CloseButton : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly HoverLayer hover;
        private readonly FlashLayer flash;

        public CloseButton()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.Centre;
            CornerRadius = 6;
            Masking = true;
            Children =
            [
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                new FluXisSpriteIcon
                {
                    Icon = Phosphor.Bold.X,
                    Size = new Vector2(14),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            ];
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

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.Show();
            return base.OnClick(e);
        }
    }
}
