using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Setup;
using fluXis.Screens.Edit.Tabs.Setup.Entries;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.Tabs;

public partial class SetupTab : EditorTab
{
    public override IconUsage Icon => FontAwesome6.Solid.ScrewdriverWrench;
    public override string TabName => "Setup";

    private SetupSection metadata;

    [BackgroundDependencyLoader]
    private void load(EditorMap map)
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(30),
                    Padding = new MarginPadding { Horizontal = 200, Vertical = 50 },
                    Children = new Drawable[]
                    {
                        new SetupHeader(),
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding { Horizontal = 40 },
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                ColumnDimensions = new[]
                                {
                                    new Dimension(),
                                    new Dimension(GridSizeMode.Absolute, 20),
                                    new Dimension()
                                },
                                RowDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize),
                                },
                                Content = new[]
                                {
                                    new[]
                                    {
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(30),
                                            Children = new Drawable[]
                                            {
                                                metadata = new SetupSection("Metadata")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupTextBox("Title")
                                                        {
                                                            Default = map.MapInfo.Metadata.Title,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.Title = map.RealmMap.Metadata.Title = value
                                                        },
                                                        new SetupTextBox("Title (Romanized)")
                                                        {
                                                            Default = map.MapInfo.Metadata.TitleRomanized ?? map.MapInfo.Metadata.Title,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.TitleRomanized = map.RealmMap.Metadata.TitleRomanized = value
                                                        },
                                                        new SetupTextBox("Artist")
                                                        {
                                                            Default = map.MapInfo.Metadata.Artist,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.Artist = map.RealmMap.Metadata.Artist = value
                                                        },
                                                        new SetupTextBox("Artist (Romanized)")
                                                        {
                                                            Default = map.MapInfo.Metadata.ArtistRomanized ?? map.MapInfo.Metadata.Artist,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.ArtistRomanized = map.RealmMap.Metadata.ArtistRomanized = value
                                                        },
                                                        new SetupTextBox("Mapper")
                                                        {
                                                            Default = map.MapInfo.Metadata.Mapper,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.Mapper = map.RealmMap.Metadata.Mapper = value
                                                        },
                                                        new SetupTextBox("Difficulty")
                                                        {
                                                            Default = map.MapInfo.Metadata.Difficulty,
                                                            Placeholder = "...",
                                                            OnChange = value => map.MapInfo.Metadata.Difficulty = map.RealmMap.Difficulty = value
                                                        },
                                                        new SetupTextBox("Tags")
                                                        {
                                                            Default = map.MapInfo.Metadata.Tags,
                                                            Placeholder = "No Tags",
                                                            OnChange = value => map.MapInfo.Metadata.Tags = map.RealmMap.Metadata.Tags = value,
                                                            MaxLength = 2048
                                                        }
                                                    }
                                                },
                                                new SetupSection("Colors")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupColor("Accent")
                                                        {
                                                            Color = map.RealmMap.Metadata.Color,
                                                            OnColorChanged = color => map.MapInfo.Colors.Accent = map.RealmMap.Metadata.Color = color
                                                        },
                                                        new SetupColor("Primary")
                                                        {
                                                            Color = map.MapInfo.Colors.GetColor(1, Colour4.White),
                                                            OnColorChanged = color => map.MapInfo.Colors.PrimaryHex = color.ToHex()
                                                        },
                                                        new SetupColor("Secondary")
                                                        {
                                                            Color = map.MapInfo.Colors.GetColor(2, Colour4.White),
                                                            OnColorChanged = color => map.MapInfo.Colors.SecondaryHex = color.ToHex()
                                                        },
                                                        new SetupColor("Middle")
                                                        {
                                                            Color = map.MapInfo.Colors.GetColor(3, Colour4.White),
                                                            OnColorChanged = color => map.MapInfo.Colors.MiddleHex = color.ToHex()
                                                        }
                                                    }
                                                },
                                                new SetupSection("Special")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupToggle("New Lane Switch Layout", map.MapInfo.NewLaneSwitchLayout)
                                                        {
                                                            TooltipText = "Improves the 6k and 8k layouts for lane switches",
                                                            OnChange = value =>
                                                            {
                                                                map.MapInfo.NewLaneSwitchLayout = value;
                                                                map.MapEvents.LaneSwitchEvents.ForEach(map.Update);
                                                            }
                                                        },
                                                        new SetupSlider<int>("Extra Playfields", 0, 5, 1)
                                                        {
                                                            Default = map.MapInfo.ExtraPlayfields,
                                                            Format = "0",
                                                            OnChange = value => map.MapInfo.ExtraPlayfields = value
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        Empty(),
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(30),
                                            Children = new Drawable[]
                                            {
                                                new SetupSection("Assets")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupAsset("Audio")
                                                        {
                                                            AllowedExtensions = FluXisGame.AUDIO_EXTENSIONS,
                                                            Path = map.MapInfo.AudioFile,
                                                            OnChange = map.SetAudio
                                                        },
                                                        new SetupAsset("Background")
                                                        {
                                                            AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                            Path = map.MapInfo.BackgroundFile,
                                                            OnChange = map.SetBackground
                                                        },
                                                        new SetupAsset("Cover")
                                                        {
                                                            AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                            Path = map.MapInfo.CoverFile,
                                                            OnChange = map.SetCover
                                                        },
                                                        new SetupAsset("Video")
                                                        {
                                                            AllowedExtensions = FluXisGame.VIDEO_EXTENSIONS,
                                                            Path = map.MapInfo.VideoFile,
                                                            OnChange = map.SetVideo
                                                        }
                                                    }
                                                },
                                                new SetupSection("Sources")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupTextBox("Audio")
                                                        {
                                                            Default = map.MapInfo.Metadata.AudioSource,
                                                            Placeholder = "No Source",
                                                            OnChange = value => map.MapInfo.Metadata.AudioSource = map.RealmMap.Metadata.Source = value
                                                        },
                                                        new SetupTextBox("Background")
                                                        {
                                                            Default = map.MapInfo.Metadata.BackgroundSource,
                                                            Placeholder = "No Source",
                                                            OnChange = value => map.MapInfo.Metadata.BackgroundSource = value
                                                        },
                                                        new SetupTextBox("Cover")
                                                        {
                                                            Default = map.MapInfo.Metadata.CoverSource,
                                                            Placeholder = "No Source",
                                                            OnChange = value => map.MapInfo.Metadata.CoverSource = value
                                                        },
                                                    }
                                                },
                                                new SetupSection("Keymode")
                                                {
                                                    Entries = new Drawable[] { new SetupKeymode() }
                                                },
                                                new SetupSection("Difficulty")
                                                {
                                                    Entries = new Drawable[]
                                                    {
                                                        new SetupSlider<float>("Accuracy", 1, 10, 0.1f)
                                                        {
                                                            Default = map.MapInfo.AccuracyDifficulty,
                                                            OnChange = value => map.MapInfo.AccuracyDifficulty = map.RealmMap.AccuracyDifficulty = value
                                                        },
                                                        new SetupSlider<float>("Health", 1, 10, 0.1f)
                                                        {
                                                            Default = map.MapInfo.HealthDifficulty,
                                                            OnChange = value => map.MapInfo.HealthDifficulty = map.RealmMap.HealthDifficulty = value
                                                        },
                                                    }
                                                },
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

        // tabbing support
        metadata.OfType<SetupTextBox>().ForEach(textBox => textBox.TabbableContentContainer = metadata);
    }
}
