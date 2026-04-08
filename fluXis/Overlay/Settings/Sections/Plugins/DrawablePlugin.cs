using fluXis.Database;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Import;
using fluXis.Plugins;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Settings.Sections.Plugins;

public partial class DrawablePlugin : Container
{
    public Plugin Plugin { get; init; }
    protected FillFlowContainer Flow;

    [Resolved]
    protected FluXisRealm Realm { get; private set; }

    [Resolved]
    protected ImportManager ImportManager { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            Flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(10),
                Spacing = new Vector2(10),
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Children = new[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = Plugin.Name,
                                        FontSize = 24
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"by {Plugin.Author}",
                                        Colour = Theme.Text2,
                                        FontSize = 16
                                    }
                                }
                            },
                            new FluXisSpriteText
                            {
                                Text = Plugin.Version.ToString(),
                                FontSize = 16,
                                Colour = Theme.Text2,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
                            }
                        }
                    }
                }
            }
        };

        AfterLoad();
        Flow.AddRange(Plugin.CreateSettings());
    }

    protected virtual void AfterLoad() { }
}

