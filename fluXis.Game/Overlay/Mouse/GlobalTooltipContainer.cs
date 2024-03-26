using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
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
        private FluXisSpriteText text { get; }

        public TextTooltip()
        {
            CornerRadius = 5;
            Masking = true;
            AutoSizeEasing = Easing.OutQuint;
            EdgeEffect = FluXisStyles.ShadowSmall;

            Child = text = new FluXisSpriteText
            {
                WebFontSize = 16,
                Padding = new MarginPadding { Horizontal = 10, Vertical = 6 }
            };
        }

        public override void SetContent(LocalisableString content)
        {
            if (content == text.Text)
                return;

            text.Text = content;
            AutoSizeDuration = IsPresent ? 250 : 0;
        }
    }
}
