using System.IO;
using System.Linq;
using fluXis.Audio.FFT;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Form;
using fluXis.Screens.Edit.Tabs.Setup;
using fluXis.Screens.Edit.Tabs.Setup.Entries;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.Tabs;

public partial class SetupTab : EditorTab
{
    public override IconUsage Icon => Phosphor.Bold.Wrench;
    public override string TabName => "Setup";

    private SetupSection metadata;

    [Resolved]
    private AudioAnalyzer analyzer { get; set; }

    [BackgroundDependencyLoader]
    private void load(EditorMap map)
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(32),
                    Padding = new MarginPadding { Horizontal = 196, Vertical = 48 },
                    Children = new Drawable[]
                    {
                        new SetupHeader(),
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding { Horizontal = 24 },
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                ColumnDimensions = new[]
                                {
                                    new Dimension(),
                                    new Dimension(GridSizeMode.Absolute, 16),
                                    new Dimension()
                                },
                                RowDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize)
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
                                            Spacing = new Vector2(20),
                                            Children = new Drawable[]
                                            {
                                                metadata = new SetupSection("Metadata", [
                                                    new FormInput("Title", map.MapInfo.Metadata.Title)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.Title = map.RealmMap.Metadata.Title = v
                                                    },
                                                    new FormInput("Title (Romanized)", map.MapInfo.Metadata.TitleRomanized ?? map.MapInfo.Metadata.Title)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.TitleRomanized = map.RealmMap.Metadata.TitleRomanized = v
                                                    },
                                                    new FormInput("Artist", map.MapInfo.Metadata.Artist)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.Artist = map.RealmMap.Metadata.Artist = v
                                                    },
                                                    new FormInput("Artist (Romanized)", map.MapInfo.Metadata.ArtistRomanized ?? map.MapInfo.Metadata.Artist)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.ArtistRomanized = map.RealmMap.Metadata.ArtistRomanized = v
                                                    },
                                                    new FormInput("Mapper", map.MapInfo.Metadata.Mapper)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.Mapper = map.RealmMap.Metadata.Mapper = v
                                                    },
                                                    new FormInput("Difficulty", map.MapInfo.Metadata.Difficulty)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.Difficulty = map.RealmMap.Difficulty = v
                                                    },
                                                    new FormInput("Tags", map.MapInfo.Metadata.Tags)
                                                    {
                                                        Placeholder = "No Tags",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.Tags = map.RealmMap.Metadata.Tags = v,
                                                        MaxLength = 2048
                                                    }
                                                ]),
                                                new SetupSection("Colors", [
                                                    new SetupSection.Row([
                                                        new FormColor("Accent", map.RealmMap.Metadata.Color)
                                                        {
                                                            OnValueChanged = (_, v) => map.MapInfo.Colors.Accent = map.RealmMap.Metadata.Color = v
                                                        },
                                                        new FormColor("Primary", map.MapInfo.Colors.GetColor(1, Colour4.White))
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.MapInfo.Colors.PrimaryHex = v.ToHex();
                                                                map.TriggerAnyChange();
                                                            }
                                                        }
                                                    ]),
                                                    new SetupSection.Row([
                                                        new FormColor("Secondary", map.MapInfo.Colors.GetColor(2, Colour4.White))
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.MapInfo.Colors.SecondaryHex = v.ToHex();
                                                                map.TriggerAnyChange();
                                                            }
                                                        },
                                                        new FormColor("Middle", map.MapInfo.Colors.GetColor(3, Colour4.White))
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.MapInfo.Colors.MiddleHex = v.ToHex();
                                                                map.TriggerAnyChange();
                                                            }
                                                        }
                                                    ])
                                                ]),
                                                new SetupSection("Special", [
                                                    new SetupSection.Row([
                                                        new FormCheckbox("Force 16:9 Aspect Ratio", map.MapInfo.Force16By9)
                                                        {
                                                            OnValueChanged = (_, v) => map.MapInfo.Force16By9 = v
                                                        },
                                                        new FormCheckbox("New Lane Switch Layout", map.MapInfo.NewLaneSwitchLayout)
                                                        {
                                                            Description = "Improves the 6k and 8k layouts for lane switches",
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.MapInfo.NewLaneSwitchLayout = v;
                                                                map.MapEvents.LaneSwitchEvents.ForEach(map.Update);
                                                            }
                                                        }
                                                    ]),
                                                    new SetupSection.Row([
                                                        new FormCheckbox("Enable Visualization", map.MapInfo.EnableVisualization)
                                                        {
                                                            Description = "Allows getting audio amplitude data in scripts",
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.MapInfo.EnableVisualization = v;

                                                                // immediately start fft processing
                                                                if (v) analyzer.SetAudio(map.RealmMap);
                                                            }
                                                        },
                                                        new FormSlider<int>("Extra Playfields", map.MapInfo.ExtraPlayfields, 0, 9)
                                                        {
                                                            OnValueChanged = (_, v) => map.MapInfo.ExtraPlayfields = v
                                                        }
                                                    ])
                                                ])
                                            }
                                        },
                                        Empty(),
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(20),
                                            Children = new Drawable[]
                                            {
                                                new SetupSection("Assets", [
                                                    new FormBasicFilePicker("Audio", map.MapInfo.AudioFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.AUDIO_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetAudio(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Background", map.MapInfo.BackgroundFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetBackground(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Cover", map.MapInfo.CoverFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetCover(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Video", map.MapInfo.VideoFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.VIDEO_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetVideo(new FileInfo(v))
                                                    }
                                                ]),
                                                new SetupSection("Sources", [
                                                    new FormInput("Audio", map.MapInfo.Metadata.AudioSource)
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.AudioSource = map.RealmMap.Metadata.Source = v
                                                    },
                                                    new FormInput("Background", map.MapInfo.Metadata.BackgroundSource)
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.BackgroundSource = v
                                                    },
                                                    new FormInput("Cover", map.MapInfo.Metadata.CoverSource)
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.MapInfo.Metadata.CoverSource = v
                                                    }
                                                ]),
                                                new SetupSection("Keymode", [new SetupKeymode()]),
                                                new SetupSection("Difficulty", [
                                                    new FormSlider<float>("Accuracy", new BindableNumber<float>(8)
                                                    {
                                                        Value = map.MapInfo.AccuracyDifficulty,
                                                        MinValue = 1, MaxValue = 10, Precision = 0.1f
                                                    }) { OnValueChanged = (_, v) => map.MapInfo.AccuracyDifficulty = map.RealmMap.AccuracyDifficulty = v },
                                                    new FormSlider<float>("Health", new BindableNumber<float>(8)
                                                    {
                                                        Value = map.MapInfo.HealthDifficulty,
                                                        MinValue = 1, MaxValue = 10, Precision = 0.1f
                                                    }) { OnValueChanged = (_, v) => map.MapInfo.HealthDifficulty = map.RealmMap.HealthDifficulty = v }
                                                ])
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
        metadata.OfType<FormInput>().ForEach(i => i.TabbableContentContainer = metadata);
    }
}
