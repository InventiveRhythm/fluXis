using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Network;

public abstract partial class DashboardTab : Container
{
    public abstract LocalisableString Title { get; }
    public abstract IconUsage Icon { get; }
    public abstract DashboardTabType Type { get; }

    protected Container Header { get; private set; }
    protected new Container Content { get; private set; }

    private const int title_height = 50;
    private const int icon_size = 26;

    protected DashboardTab()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension()
                },
                ColumnDimensions = new[]
                {
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = title_height,
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Children = new Drawable[]
                                    {
                                        new FluXisSpriteIcon
                                        {
                                            Icon = Icon,
                                            Size = new Vector2(icon_size),
                                            Margin = new MarginPadding((title_height - icon_size) / 2f),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = Title,
                                            FontSize = 22,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft
                                        }
                                    }
                                },
                                Header = new Container { RelativeSizeAxes = Axes.Both }
                            }
                        }
                    },
                    new Drawable[]
                    {
                        Content = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding(20)
                        }
                    }
                }
            }
        };
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
}

public enum DashboardTabType
{
    Notifications,
    News,
    Friends,
    Club,
    Online,
    Account
}
