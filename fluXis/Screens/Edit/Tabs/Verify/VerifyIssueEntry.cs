using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Verify;

public partial class VerifyIssueEntry : CompositeDrawable
{
    private VerifyIssue issue { get; }

    private HoverLayer hover;

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
            VerifyIssueSeverity.Hint => FluXisColors.Blue,
            VerifyIssueSeverity.Warning => FluXisColors.Yellow,
            VerifyIssueSeverity.Problematic => FluXisColors.Red,
            _ => FluXisColors.Background2
        };

        InternalChildren = new Drawable[]
        {
            hover = new HoverLayer { Colour = color },
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

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }
}
