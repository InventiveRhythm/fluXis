using osu.Framework.Platform;
using Markdig.Syntax;
using fluXis.Overlay.Toolbar;
using fluXis.Localization;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Input.Events;
using osu.Framework.Allocation;
using fluXis.Overlay.Notifications;

public partial class FluXisMarkdownCopyButton : ToolbarButton
{
    [Resolved]
    private Clipboard clipboard { get; set; }

    private CodeBlock codeBlock;

    private NotificationManager notifications;

    public FluXisMarkdownCopyButton(CodeBlock codeBlock)
    {
        this.codeBlock = codeBlock;

        RequireLogin = false;
        TooltipTitle = LocalizationStrings.General.CopyToClipboard;
        TooltipSub = LocalizationStrings.General.CopyToClipboardDescription;
        Icon = FontAwesome6.Solid.Clipboard;
    }

    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        this.notifications = notifications;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.Value = true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        clipboard.SetText(string.Join("\n", codeBlock.Lines));
        notifications.SendSmallText("Copied text to Clipboard", FontAwesome6.Solid.Clipboard);
        return base.OnClick(e);
    }
}