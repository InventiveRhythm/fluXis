using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Screens.Edit.Tabs.Metadata;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class SetupTab : EditorTab
{
    public override IconUsage Icon => FontAwesome6.Solid.ScrewdriverWrench;
    public override string TabName => "Setup";

    [Resolved]
    private EditorMap map { get; set; }

    private MapBackground background;
    private MapCover cover;
    private TruncatingText titleText;
    private TruncatingText artistText;

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new FluXisScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = 300 },
                        Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new AssetsSetupSection(),
                                new MetadataSetupSection(),
                                new DifficultySetupSection(),
                                new KeyModeSetupSection()
                            }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 300,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            background = new MapBackground(map.RealmMap)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = 0.25f
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 150,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Padding = new MarginPadding { Horizontal = 220 },
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        Size = new Vector2(150),
                                        CornerRadius = 20,
                                        Masking = true,
                                        EdgeEffect = FluXisStyles.ShadowMedium,
                                        Child = cover = new MapCover(map.MapSet)
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Direction = FillDirection.Vertical,
                                        Padding = new MarginPadding { Left = 170 },
                                        Children = new Drawable[]
                                        {
                                            titleText = new TruncatingText
                                            {
                                                Text = map.RealmMap.Metadata.Title,
                                                RelativeSizeAxes = Axes.X,
                                                FontSize = 38,
                                                Shadow = true
                                            },
                                            artistText = new TruncatingText
                                            {
                                                Text = map.RealmMap.Metadata.Artist,
                                                RelativeSizeAxes = Axes.X,
                                                FontSize = 24,
                                                Shadow = true
                                            }
                                        }
                                    }
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

        map.BackgroundChanged += () => background.Map = map.RealmMap;
        map.CoverChanged += () => cover.MapSet = map.RealmMap.MapSet;
    }

    protected override void Update()
    {
        titleText.Text = map.MapInfo.Metadata.Title;
        artistText.Text = map.MapInfo.Metadata.Artist;
    }
}
