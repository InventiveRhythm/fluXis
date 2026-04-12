using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Overlay.Notifications;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Platform;

namespace fluXis.Screens.Edit.Tabs.Verify;

public partial class VerifyIssueEntry : CompositeDrawable
{
    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private VerifyIssue issue { get; }

    private HoverLayer hover;
    private FlashLayer flash;

    public VerifyIssueEntry(VerifyIssue issue)
    {
        this.issue = issue;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 28;
        CornerRadius = 8;
        Masking = true;

        var color = issue.Severity switch
        {
            VerifyIssueSeverity.Hint => Theme.Blue,
            VerifyIssueSeverity.Warning => Theme.Yellow,
            VerifyIssueSeverity.Problematic => Theme.Red,
            _ => Theme.Background2
        };

        InternalChildren = new Drawable[]
        {
            hover = new HoverLayer { Colour = color },
            flash = new FlashLayer { Colour = color.Lighten(.5f).Opacity(.5f) },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 8 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color,
                    ColumnDimensions = VerifyTab.COLUMNS.Select(x =>
                    {
                        var size = x.Item2;
                        return size == 0 ? new Dimension() : new Dimension(GridSizeMode.Absolute, size);
                    }).ToArray(),
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new TruncatingText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = issue.Severity.ToString(),
                                WebFontSize = 12,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new TruncatingText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = issue.Category.ToString(),
                                WebFontSize = 12,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new TruncatingText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = issue.Time == null ? "-" : TimeUtils.Format(issue.Time.Value),
                                WebFontSize = 12,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new TruncatingText
                            {
                                RelativeSizeAxes = Axes.X,
                                Text = issue.Message,
                                WebFontSize = 12,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (issue.Time == null) return false;

        clipboard.SetText(((int)issue.Time).ToString());
        notifications.SendSmallText("Copied issue time to clipboard.");

        samples.Click();
        flash.Show();

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (issue.Time != null) samples.Hover();
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
