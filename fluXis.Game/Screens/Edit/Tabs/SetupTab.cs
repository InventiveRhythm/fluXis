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
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class SetupTab : EditorTab
{
    [Resolved]
    private EditorValues values { get; set; }

    private MapBackground background;
    private MapCover cover;
    private FluXisSpriteText titleText;
    private FluXisSpriteText artistText;

    public SetupTab(Editor screen)
        : base(screen)
    {
    }

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
                                new AssetsSetupSection
                                {
                                    BackgroundChanged = () => background.Map = Screen.Map,
                                    CoverChanged = () => cover.MapSet = Screen.Map.MapSet
                                },
                                new MetadataSetupSection(values.MapInfo.Metadata),
                                new DifficultySetupSection(),
                                new KeyModeSetupSection(Screen.Map)
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
                            background = new MapBackground(Screen.Map)
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
                                        Child = cover = new MapCover(Screen.Map.MapSet)
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
                                            titleText = new FluXisSpriteText
                                            {
                                                Text = Screen.Map.Metadata.Title,
                                                RelativeSizeAxes = Axes.X,
                                                Truncate = true,
                                                FontSize = 38,
                                                Shadow = true
                                            },
                                            artistText = new FluXisSpriteText
                                            {
                                                Text = Screen.Map.Metadata.Artist,
                                                RelativeSizeAxes = Axes.X,
                                                Truncate = true,
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

    protected override void Update()
    {
        titleText.Text = values.MapInfo.Metadata.Title;
        artistText.Text = values.MapInfo.Metadata.Artist;
    }
}
