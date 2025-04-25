using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Screens.Edit.Tabs.Verify;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Screens.Edit.Tabs;

public partial class VerifyTab : EditorTab
{
    public override IconUsage Icon => FontAwesome.Solid.CheckDouble;
    public override string TabName => "Verify";

    public static readonly List<(string, float)> COLUMNS = new()
    {
        ("Severity", 120),
        ("Category", 120),
        ("Time", 90),
        ("Message", 0)
    };

    [Resolved]
    private EditorMap map { get; set; }

    private ForcedHeightText totalIssuesText;
    private ForcedHeightText problematicIssuesText;
    private FillFlowContainer<VerifyIssueEntry> issuesContainer;

    private readonly List<IVerifyCheck> checks = new();

    private readonly List<VerifyIssue> issues = new();
    public IReadOnlyList<VerifyIssue> Issues => issues;

    public int TotalIssues => issues.Count;
    public int ProblematicIssues => issues.Count(x => x.Severity == VerifyIssueSeverity.Problematic);

    [BackgroundDependencyLoader]
    private void load()
    {
        loadChecks();

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(40),
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new Dimension[]
                    {
                        new(GridSizeMode.Absolute, 40), // header
                        new(GridSizeMode.Absolute, 20), // spacing
                        new(GridSizeMode.Absolute, 8), // titles
                        new(GridSizeMode.Absolute, 8), // spacing
                        new() // scroll content
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(8),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Children = new Drawable[]
                                        {
                                            totalIssuesText = new ForcedHeightText
                                            {
                                                Text = "{} total issues",
                                                WebFontSize = 20,
                                                Height = 15
                                            },
                                            problematicIssuesText = new ForcedHeightText
                                            {
                                                Text = "{} problematic",
                                                WebFontSize = 14,
                                                Height = 10,
                                                Alpha = .6f
                                            }
                                        }
                                    },
                                    new FluXisButton
                                    {
                                        Size = new Vector2(112, 40),
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Text = "Refresh",
                                        Action = RefreshIssues
                                    }
                                }
                            }
                        },
                        new[] { Empty() },
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Horizontal = 8 },
                                Child = new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    ColumnDimensions = COLUMNS.Select(x =>
                                    {
                                        var size = x.Item2;
                                        return size == 0 ? new Dimension() : new Dimension(GridSizeMode.Absolute, size);
                                    }).ToArray(),
                                    Content = new[]
                                    {
                                        COLUMNS.Select(x =>
                                        {
                                            var text = x.Item1;

                                            return new TruncatingText
                                            {
                                                Text = text,
                                                RelativeSizeAxes = Axes.X,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                WebFontSize = 10
                                            };
                                        }).ToArray()
                                    }
                                }
                            }
                        },
                        new[] { Empty() },
                        new Drawable[]
                        {
                            new FluXisScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarVisible = false,
                                Child = issuesContainer = new FillFlowContainer<VerifyIssueEntry>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Spacing = new Vector2(4)
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        RefreshIssues();
    }

    private void loadChecks()
    {
        var assembly = GetType().Assembly;
        var types = assembly.GetTypes()
                            .Where(t => t.IsClass && !t.IsAbstract && typeof(IVerifyCheck).IsAssignableFrom(t))
                            .ToList();

        foreach (var type in types)
        {
            try
            {
                var check = (IVerifyCheck)Activator.CreateInstance(type);
                if (check != null) checks.Add(check);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to create instance of {type.Name}: {ex.Message}", LoggingTarget.Runtime, LogLevel.Error);
            }
        }
    }

    public void RefreshIssues()
    {
        issues.Clear();
        issues.AddRange(checks.SelectMany(x => x.Check(map)));
        issues.Sort((a, b) =>
        {
            if (a.Time != b.Time)
                return a.Time?.CompareTo(b.Time) ?? 0;

            if (a.Severity != b.Severity)
                return a.Severity.CompareTo(b.Severity);

            return string.Compare(a.Message, b.Message, StringComparison.Ordinal);
        });

        issuesContainer.Clear();
        issuesContainer.AddRange(issues.Select(x => new VerifyIssueEntry(x)));

        totalIssuesText.Text = $"{TotalIssues} total issues";
        problematicIssuesText.Text = $"{ProblematicIssues} problematic";
    }
}
