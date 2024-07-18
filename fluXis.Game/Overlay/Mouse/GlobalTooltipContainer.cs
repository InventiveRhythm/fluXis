using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Mouse;

public partial class GlobalTooltipContainer : TooltipContainer
{
    protected override double AppearDelay => (1 - CurrentTooltip.Alpha) * base.AppearDelay;

    protected override ITooltip CreateTooltip() => new TextTooltip();

    public GlobalTooltipContainer(CursorContainer cursor)
        : base(cursor)
    {
    }

    public partial class TextTooltip : CustomTooltipContainer<LocalisableString>
    {
        private FluXisTextFlow text { get; }
        private LocalisableString currentText;

        public TextTooltip()
        {
            CornerRadius = 5;
            Masking = true;
            AutoSizeEasing = Easing.OutQuint;
            EdgeEffect = FluXisStyles.ShadowSmall;

            Child = text = new FluXisTextFlow()
            {
                AutoSizeAxes = Axes.Both,
                WebFontSize = 16,
                Padding = new MarginPadding { Horizontal = 10, Vertical = 6 }
            };
        }

        public override void SetContent(LocalisableString content)
        {
            if (content == currentText)
                return;

            text.Text = currentText = content;
            AutoSizeDuration = IsPresent ? 250 : 0;
        }
    }
}
