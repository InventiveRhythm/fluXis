using System;
using System.IO;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Files;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class AssetsSetupSection : SetupSection
{
    [Resolved]
    private EditorValues values { get; set; }

    public Action BackgroundChanged { get; set; }
    public Action CoverChanged { get; set; }

    private AssetsSetupTextBox audioTextBox;
    private AssetsSetupTextBox backgroundTextBox;
    private AssetsSetupTextBox coverTextBox;
    private AssetsSetupTextBox videoTextBox;

    public AssetsSetupSection()
        : base("Assets")
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            audioTextBox = new AssetsSetupTextBox
            {
                Label = "Audio",
                AllowedExtensions = FluXisGame.AUDIO_EXTENSIONS,
                FileName = values.MapInfo.AudioFile,
                OnFileSelected = file =>
                {
                    values.Editor.SetAudio(file);
                    audioTextBox!.FileName = values.MapInfo.AudioFile;
                }
            },
            backgroundTextBox = new AssetsSetupTextBox
            {
                Label = "Background",
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                FileName = values.MapInfo.BackgroundFile,
                OnFileSelected = file =>
                {
                    values.Editor.SetBackground(file);
                    BackgroundChanged?.Invoke();
                    backgroundTextBox!.FileName = values.MapInfo.BackgroundFile;
                }
            },
            coverTextBox = new AssetsSetupTextBox
            {
                Label = "Cover",
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS,
                FileName = values.MapInfo.CoverFile,
                OnFileSelected = file =>
                {
                    values.Editor.SetCover(file);
                    CoverChanged?.Invoke();
                    coverTextBox!.FileName = values.MapInfo.CoverFile;
                }
            },
            videoTextBox = new AssetsSetupTextBox
            {
                Label = "Video",
                AllowedExtensions = FluXisGame.VIDEO_EXTENSIONS,
                FileName = values.MapInfo.VideoFile,
                OnFileSelected = file =>
                {
                    values.Editor.SetVideo(file);
                    videoTextBox!.FileName = values.MapInfo.VideoFile;
                }
            }
        });
    }

    private partial class AssetsSetupTextBox : Container, IDragDropHandler
    {
        public string[] AllowedExtensions { get; init; }

        [Resolved]
        private FluXisGameBase game { get; set; }

        [Resolved]
        private EditorValues values { get; set; }

        [Resolved]
        private Storage storage { get; set; }

        public string Label { get; init; }
        public Action<FileInfo> OnFileSelected { get; init; }

        private string fileName;

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;

                if (clickText is null) return;

                clickText.Text = string.IsNullOrEmpty(fileName) ? "Click to select file..." : fileName;
                clickText.Colour = string.IsNullOrEmpty(FileName) ? FluXisColors.Text2 : FluXisColors.Text;
            }
        }

        private FluXisSpriteText clickText;

        private InputManager inputManager;

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
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreRight,
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
                                Colour = FluXisColors.Background3
                            },
                            clickText = new FluXisSpriteText
                            {
                                Text = string.IsNullOrEmpty(FileName) ? "Click to select file..." : FileName,
                                Colour = string.IsNullOrEmpty(FileName) ? FluXisColors.Text2 : FluXisColors.Text,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Padding = new MarginPadding { Left = 8 }
                            }
                        }
                    },
                    Padding = new MarginPadding { Left = 150 },
                    Action = () =>
                    {
                        game.Overlay = new FileSelect
                        {
                            AllowedExtensions = AllowedExtensions,
                            MapDirectory = storage.GetFullPath($"maps/{values.Editor.Map.MapSet.ID}"),
                            OnFileSelected = file => OnFileSelected?.Invoke(file)
                        };
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            game.AddDragDropHandler(this);
            inputManager = GetContainingInputManager();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            game.RemoveDragDropHandler(this);
        }

        public bool OnDragDrop(string file)
        {
            var pos = inputManager.CurrentState.Mouse.Position;
            var quad = ScreenSpaceDrawQuad;

            Logger.Log($"pos: {pos}, quad: {quad}", LoggingTarget.Runtime, LogLevel.Debug);

            if (!quad.Contains(pos)) return false;

            Schedule(() => OnFileSelected?.Invoke(new FileInfo(file)));

            return true;
        }
    }
}
