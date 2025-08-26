using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Browse;

public partial class BrowseFilter<T> : FillFlowContainer
{
    public BrowseFilter(LocalisableString title, IEnumerable<Option> options)
    {
        RelativeSizeAxes = Axes.X;
        Height = 20;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4);

        Children = new Drawable[]
        {
            new TruncatingText
            {
                Text = title,
                Width = 80,
                WebFontSize = 12,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            }
        };

        options.ForEach(x => Add(new Button(x)));
    }

    public class Option
    {
        public T Value { get; }
        public LocalisableString Text { get; }
        public Colour4 Color { get; }

        public Option(T value, LocalisableString text, Colour4 color)
        {
            Value = value;
            Text = text;
            Color = color;
        }
    }

    private partial class Button : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private readonly Option option;

        private Box background;
        private HoverLayer hover;
        private FlashLayer flash;
        private FluXisSpriteText text;

        private bool enabled;

        public Button(Option option)
        {
            this.option = option;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.X;
            Height = 20;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            CornerRadius = 4;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = option.Color,
                    Alpha = 0
                },
                hover = new HoverLayer(),
                flash = new FlashLayer(),
                text = new FluXisSpriteText
                {
                    Text = option.Text,
                    WebFontSize = 12,
                    Margin = new MarginPadding { Horizontal = 8 },
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = .6f
                }
            };
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
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            base.OnMouseUp(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            enabled = !enabled;
            samples.Click();
            flash.Show();

            background.Alpha = enabled ? 1f : 0f;
            text.Alpha = enabled ? 1f : .6f;
            text.Colour = enabled ? Theme.TextDark : Theme.Text;

            return true;
        }
    }
}
