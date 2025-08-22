using osu.Framework.Platform;
using Markdig.Syntax;
using fluXis.Localization;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Input.Events;
using osu.Framework.Allocation;
using fluXis.Overlay.Notifications;
using fluXis.Graphics.UserInterface.Buttons;
using osu.Framework.Graphics.Cursor;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osuTK;
using fluXis.Overlay.Mouse;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownCopyButton : IconButton, IHasCustomTooltip<FluXisMarkdownCopyButton>
{
    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private readonly CodeBlock codeBlock;
    private readonly CopyButtonTooltip tooltip;

    public FluXisMarkdownCopyButton TooltipContent => this;

    public FluXisMarkdownCopyButton(CodeBlock codeBlock)
    {
        this.codeBlock = codeBlock;
        tooltip = new CopyButtonTooltip();

        Icon = FontAwesome6.Solid.Clipboard;
        IconSize = 24;
        ButtonSize = 46;
    }

    protected override bool OnClick(ClickEvent e)
    {
        clipboard.SetText(string.Join("\n", codeBlock.Lines));
        notifications.SendSmallText("Copied text to Clipboard", FontAwesome6.Solid.Clipboard);
        return base.OnClick(e);
    }

    public ITooltip<FluXisMarkdownCopyButton> GetCustomTooltip() => tooltip;

    private partial class CopyButtonTooltip : CustomTooltipContainer<FluXisMarkdownCopyButton>
    {
        public CopyButtonTooltip()
        {
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Margin = new MarginPadding { Horizontal = 10, Vertical = 6 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Size = new Vector2(16),
                                Margin = new MarginPadding(4),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Icon = FontAwesome6.Solid.Clipboard
                            },
                            new FluXisSpriteText
                            {
                                FontSize = 24,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = LocalizationStrings.General.CopyToClipboard
                            }
                        }
                    },
                    new FluXisTextFlow
                    {
                        AutoSizeAxes = Axes.Both,
                        FontSize = 18,
                        Text = LocalizationStrings.General.CopyToClipboardDescription
                    }
                }
            };
        }

        public override void SetContent(FluXisMarkdownCopyButton content) {} // Already set in constuctor so, this is pretty much implemented for the sake of it.
    }
}