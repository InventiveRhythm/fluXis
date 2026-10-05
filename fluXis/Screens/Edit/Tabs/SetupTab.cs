using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Audio.FFT;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Form;
using fluXis.Map.Structures.Events;
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
using osuTK.Graphics;

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
                                                    new FormInput("Title", map.Playable.Title)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.Title = v
                                                    },
                                                    new FormInput("Title (Romanized)", map.Playable.TitleRomanized ?? map.Playable.Title)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.TitleRomanized = v
                                                    },
                                                    new FormInput("Artist", map.Playable.Artist)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.Artist = v
                                                    },
                                                    new FormInput("Artist (Romanized)", map.Playable.ArtistRomanized ?? map.Playable.Artist)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.ArtistRomanized = v
                                                    },
                                                    new FormInput("Mapper", map.Playable.Creator)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.Creator = v
                                                    },
                                                    new FormInput("Difficulty", map.Playable.Difficulty)
                                                    {
                                                        Placeholder = "...",
                                                        OnValueChanged = (_, v) => map.Playable.Difficulty = v
                                                    },
                                                    new FormInput("Tags", string.Join(", ", map.Playable.Tags))
                                                    {
                                                        Placeholder = "No Tags",
                                                        OnValueChanged = (_, v) => map.Playable.Tags = [.. v.Split(",").Select(x => x.Trim())],
                                                        MaxLength = 2048
                                                    }
                                                ]),
                                                new SetupSection("Colors", [
                                                    new SetupSection.Row([
                                                        new FormColor("Accent", map.RealmMap.Metadata.Color)
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.Colors["$accent"] = new Colour4(v.Vector with { W = 1 });
                                                            }
                                                        },
                                                        new FormColor("Primary", map.Playable.Colors.TryGetValue("$primary", out var c1) ? c1 : Color4.White)
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.Colors["$primary"] = new Colour4(v.Vector with { W = 1 });
                                                                map.TriggerAnyChange();
                                                            }
                                                        }
                                                    ]),
                                                    new SetupSection.Row([
                                                        new FormColor("Secondary", map.Playable.Colors.TryGetValue("$secondary", out var c2) ? c2 : Color4.White)
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.Colors["$secondary"] = new Colour4(v.Vector with { W = 1 });
                                                                map.TriggerAnyChange();
                                                            }
                                                        },
                                                        new FormColor("Middle", map.Playable.Colors.TryGetValue("$middle", out var c3) ? c3 : Color4.White)
                                                        {
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.Colors["$middle"] = new Colour4(v.Vector with { W = 1 });
                                                                map.TriggerAnyChange();
                                                            }
                                                        }
                                                    ])
                                                ]),
                                                new SetupSection("Special", [
                                                    new SetupSection.Row([
                                                        new FormCheckbox("Force 16:9 Aspect Ratio", map.Playable.ForceAspect)
                                                        {
                                                            OnValueChanged = (_, v) => map.Playable.ForceAspect = v
                                                        },
                                                        new FormCheckbox("Legacy Lane Switch Layout", map.Playable.LegacyLaneSwitchLayout)
                                                        {
                                                            Description = "Reverts back to the old 6k and 8k layouts for lane switches",
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.LegacyLaneSwitchLayout = v;
                                                                map.GetObjectsOfType<LaneSwitchEvent>().ForEach(map.Update);
                                                            }
                                                        }
                                                    ]),
                                                    new SetupSection.Row([
                                                        new FormCheckbox("Enable Visualization", map.Playable.AudioVisualizations)
                                                        {
                                                            Description = "Allows getting audio amplitude data in scripts",
                                                            OnValueChanged = (_, v) =>
                                                            {
                                                                map.Playable.AudioVisualizations = v;

                                                                // immediately start fft processing
                                                                if (v) analyzer.SetAudio(map.RealmMap);
                                                            }
                                                        },
                                                        new FormSlider<int>("Extra Playfields", map.Playable.ExtraPlayfields, 0, 9)
                                                        {
                                                            OnValueChanged = (_, v) => map.Playable.ExtraPlayfields = v
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
                                                    new FormBasicFilePicker("Audio", map.Playable.AudioFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.AUDIO_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetAudio(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Background", map.Playable.BackgroundFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetBackground(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Cover", map.Playable.CoverFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetCover(new FileInfo(v))
                                                    },
                                                    new FormBasicFilePicker("Video", map.Playable.VideoFile)
                                                    {
                                                        AllowedExtensions = FluXisGame.VIDEO_EXTENSIONS,
                                                        OnValueChanged = (_, v) => map.SetVideo(new FileInfo(v))
                                                    }
                                                ]),
                                                new SetupSection("Sources", [
                                                    new FormInput("Audio", map.Playable.Sources.GetValueOrDefault("audio", ""))
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.Playable.Sources["audio"] = map.RealmMap.Metadata.Source = v
                                                    },
                                                    new FormInput("Background", map.Playable.Sources.GetValueOrDefault("background", ""))
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.Playable.Sources["background"] = v
                                                    },
                                                    new FormInput("Cover", map.Playable.Sources.GetValueOrDefault("cover", ""))
                                                    {
                                                        Placeholder = "No Source",
                                                        OnValueChanged = (_, v) => map.Playable.Sources["cover"] = v
                                                    }
                                                ]),
                                                new SetupSection("Keymode", [new SetupKeymode()]),
                                                new SetupSection("Difficulty", [
                                                    new FormSlider<float>("Accuracy", new BindableNumber<float>(8)
                                                    {
                                                        Value = map.Playable.AccuracyDifficulty,
                                                        MinValue = 1, MaxValue = 10, Precision = 0.1f
                                                    }) { OnValueChanged = (_, v) => map.Playable.AccuracyDifficulty = map.RealmMap.AccuracyDifficulty = v },
                                                    new FormSlider<float>("Health", new BindableNumber<float>(8)
                                                    {
                                                        Value = map.Playable.HealthDifficulty,
                                                        MinValue = 1, MaxValue = 10, Precision = 0.1f
                                                    }) { OnValueChanged = (_, v) => map.Playable.HealthDifficulty = map.RealmMap.HealthDifficulty = v }
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
