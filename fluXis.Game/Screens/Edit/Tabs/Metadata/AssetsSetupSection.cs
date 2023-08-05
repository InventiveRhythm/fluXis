using System;
using System.IO;
using fluXis.Game.Graphics;
using fluXis.Game.Screens.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class AssetsSetupSection : SetupSection
{
    [Resolved]
    private EditorValues values { get; set; }

    public Action BackgroundChanged { get; set; }
    public Action CoverChanged { get; set; }

    public AssetsSetupSection()
        : base("Assets")
    {
        AddRange(new Drawable[]
        {
            new AssetsSetupTextBox
            {
                Label = "Audio",
                AllowedExtensions = FluXisGame.AUDIO_EXTENSIONS,
                OnFileSelected = file => values.Editor.SetAudio(file)
            },
            new AssetsSetupTextBox
            {
                Label = "Background",
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                OnFileSelected = file =>
                {
                    values.Editor.SetBackground(file);
                    BackgroundChanged?.Invoke();
                }
            },
            new AssetsSetupTextBox
            {
                Label = "Cover",
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                OnFileSelected = file =>
                {
                    values.Editor.SetCover(file);
                    CoverChanged?.Invoke();
                }
            },
            new AssetsSetupTextBox
            {
                Label = "Video",
                AllowedExtensions = FluXisGame.VIDEO_EXTENSIONS,
                OnFileSelected = file => values.Editor.SetVideo(file)
            }
        });
    }

    private partial class AssetsSetupTextBox : Container
    {
        [Resolved]
        private EditorValues values { get; set; }

        public string Label { get; init; }
        public Action<FileInfo> OnFileSelected { get; init; }
        public string[] AllowedExtensions { get; init; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = Label,
                    FontSize = 24,
                    Margin = new MarginPadding { Top = 5 },
                    Origin = Anchor.TopRight,
                    X = 140
                },
                new ClickableContainer
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 30,
                    Margin = new MarginPadding { Top = 5 },
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 5,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Surface
                            },
                            new FluXisSpriteText
                            {
                                Text = "Click to select file",
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Padding = new MarginPadding { Left = 5 }
                            }
                        }
                    },
                    Padding = new MarginPadding { Left = 150 },
                    Action = () =>
                    {
                        var screen = new FileImportScreen
                        {
                            AllowedExtensions = AllowedExtensions,
                            OnFileSelected = OnFileSelected
                        };

                        values.Editor.Push(screen);
                    }
                }
            };
        }
    }
}
