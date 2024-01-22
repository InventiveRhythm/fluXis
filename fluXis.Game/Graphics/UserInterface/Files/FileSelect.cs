using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Files.Entry;
using fluXis.Game.Graphics.UserInterface.Files.Sidebar;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Files;

public partial class FileSelect : CompositeDrawable, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public bool ShowFiles { get; init; } = true;
    public string MapDirectory { get; init; } = null;
    public string[] AllowedExtensions { get; init; } = Array.Empty<string>();

    public Action<FileInfo> OnFileSelected { get; set; }
    public Action<DirectoryInfo> OnDirectorySelected { get; set; }

    private DirectoryInfo currentDirectory;
    public FileInfo CurrentFile { get; private set; }

    public event Action<FileInfo> FileChanged;

    private PathTextBox pathTextBox;
    private FluXisTextBox searchTextBox;

    private FluXisScrollContainer scrollContainer;
    private FillFlowContainer filesFlow;
    private FillFlowContainer drivesFlow;

    private FillFlowContainer noFilesContainer;
    private FillFlowContainer errorContainer;
    private FluXisSpriteText errorText;

    private Container previewContainer;

    private FluXisSpriteText fileText;
    private FluXisButton selectButton;

    private Sample errorSample { get; set; }

    public FileSelect()
    {
        Size = new Vector2(1500, 800);
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        errorSample = samples.Get("");

        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 20,
                Masking = true,
                EdgeEffect = FluXisStyles.ShadowLarge,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    }
                }
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 80),
                    new(),
                    new(GridSizeMode.Absolute, 70)
                },
                Content = new[]
                {
                    new[]
                    {
                        Empty()
                    },
                    new Drawable[]
                    {
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize, minSize: 84),
                                new Dimension(),
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new FileSelectSidebar(this, MapDirectory),
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Children = new Drawable[]
                                        {
                                            scrollContainer = new FluXisScrollContainer
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Child = new Container
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    AutoSizeAxes = Axes.Y,
                                                    Padding = new MarginPadding(20),
                                                    Children = new Drawable[]
                                                    {
                                                        filesFlow = new FillFlowContainer
                                                        {
                                                            RelativeSizeAxes = Axes.X,
                                                            AutoSizeAxes = Axes.Y,
                                                            Direction = FillDirection.Vertical,
                                                            Spacing = new Vector2(10)
                                                        },
                                                        drivesFlow = new FillFlowContainer
                                                        {
                                                            RelativeSizeAxes = Axes.X,
                                                            AutoSizeAxes = Axes.Y,
                                                            Direction = FillDirection.Vertical,
                                                            Spacing = new Vector2(10)
                                                        }
                                                    }
                                                }
                                            },
                                            noFilesContainer = new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Alpha = 0,
                                                Children = new Drawable[]
                                                {
                                                    new SpriteIcon
                                                    {
                                                        Icon = FontAwesome6.Solid.FolderOpen,
                                                        Size = new Vector2(30),
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        Margin = new MarginPadding { Bottom = 10 }
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        FontSize = 28,
                                                        Text = "No files found!"
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        Text = "Try changing the search or directory."
                                                    }
                                                }
                                            },
                                            errorContainer = new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(10),
                                                Colour = FluXisColors.Red,
                                                Alpha = 0,
                                                Children = new Drawable[]
                                                {
                                                    new SpriteIcon
                                                    {
                                                        Icon = FontAwesome6.Solid.XMark,
                                                        Size = new Vector2(30),
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre
                                                    },
                                                    errorText = new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        AutoSizeAxes = Axes.X,
                                        AutoSizeDuration = 400,
                                        AutoSizeEasing = Easing.Out,
                                        RelativeSizeAxes = Axes.Y,
                                        Masking = true,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = FluXisColors.Background3
                                            },
                                            previewContainer = new Container
                                            {
                                                AutoSizeAxes = Axes.X,
                                                RelativeSizeAxes = Axes.Y
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = FluXisColors.Background1
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(10),
                                    Children = new Drawable[]
                                    {
                                        new FluXisButton
                                        {
                                            Size = new Vector2(160, 50),
                                            FontSize = 20,
                                            Text = "Cancel",
                                            Action = Hide
                                        },
                                        fileText = new FluXisSpriteText
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            FontSize = 20
                                        },
                                        selectButton = new FluXisButton
                                        {
                                            Size = new Vector2(160, 50),
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            FontSize = 20,
                                            Text = "Select",
                                            Color = FluXisColors.Accent2,
                                            Action = onSelect,
                                            Enabled = false
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            // this needs to be layered on top of the
            // grid container because of the shadow
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 80,
                Padding = new MarginPadding { Horizontal = -50 },
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 20,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowMedium,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background3
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(10),
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.Absolute, 60),
                                    new Dimension(GridSizeMode.Absolute, 10),
                                    new Dimension(),
                                    new Dimension(GridSizeMode.Absolute, 10),
                                    new Dimension(GridSizeMode.Absolute, 400)
                                },
                                Content = new[]
                                {
                                    new[]
                                    {
                                        new OneDirUpButton
                                        {
                                            Action = () =>
                                            {
                                                if (currentDirectory == null) return;

                                                if (currentDirectory.Parent == null)
                                                {
                                                    SelectDirectory(null);
                                                    return;
                                                }

                                                SelectDirectory(currentDirectory.Parent);
                                            }
                                        },
                                        Empty(),
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            CornerRadius = 10,
                                            Masking = true,
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Colour = FluXisColors.Background2
                                                },
                                                pathTextBox = new PathTextBox()
                                            }
                                        },
                                        Empty(),
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            CornerRadius = 10,
                                            Masking = true,
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Colour = FluXisColors.Background2
                                                },
                                                new SpriteIcon
                                                {
                                                    Size = new Vector2(20),
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Icon = FontAwesome6.Solid.MagnifyingGlass,
                                                    Margin = new MarginPadding { Left = 15 }
                                                },
                                                new Container
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Padding = new MarginPadding { Left = 40, Right = 10 },
                                                    Child = searchTextBox = new FluXisTextBox
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        Height = 40,
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        PlaceholderText = "Click to search...",
                                                        BackgroundActive = FluXisColors.Background2,
                                                        BackgroundInactive = FluXisColors.Background2,
                                                        OnTextChanged = updateSearch
                                                    }
                                                }
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
        Show();

        pathTextBox.OnCommit += (_, isNew) => onPathCommit(isNew);
        changePathTo(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
    }

    private void onPathCommit(bool isNew)
    {
        if (!isNew) return;

        if (string.IsNullOrEmpty(pathTextBox.Text))
        {
            changePathTo(null);
            return;
        }

        if (!PathUtils.IsValidDirectory(pathTextBox.Text))
        {
            pathTextBox.NotifyError();
            pathTextBox.Text = currentDirectory.FullName;
            return;
        }

        changePathTo(new DirectoryInfo(pathTextBox.Text));
    }

    public void SelectFile(FileInfo file)
    {
        previewContainer.Clear();
        Logger.Log($"Selected file {file?.FullName}", LoggingTarget.Runtime, LogLevel.Debug);

        CurrentFile = file;
        fileText.Text = file?.Name ?? "";
        selectButton.Enabled = file != null;
        FileChanged?.Invoke(file);

        var type = PathUtils.GetTypeForExtension(file?.Extension);

        if (type == PathUtils.FileType.Image)
        {
            LoadComponentAsync(new FileSystemSprite(file?.FullName), sprite =>
            {
                var ratio = (float)300 / sprite.TextureWidth;

                sprite.Width = 300;
                sprite.Height = sprite.TextureHeight * ratio;

                previewContainer.Add(new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Margin = new MarginPadding(10),
                    CornerRadius = 10,
                    Masking = true,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = sprite
                });
            });
        }
    }

    public void SelectDirectory(DirectoryInfo directory) => changePathTo(directory);

    private void changePathTo(DirectoryInfo info)
    {
        Logger.Log($"Changing path to {info?.FullName ?? "drv-select"}", LoggingTarget.Runtime, LogLevel.Debug);

        scrollContainer.ScrollToStart();
        noFilesContainer.FadeOut(200);
        errorContainer.FadeOut(200);
        drivesFlow.FadeOut(200);
        filesFlow.FadeIn(200);

        currentDirectory = info;
        pathTextBox.Text = info?.FullName ?? string.Empty;

        if (info == null)
        {
            showDriveSelect();
            selectButton.Enabled = false;
            return;
        }

        if (!ShowFiles)
            selectButton.Enabled = true;

        filesFlow.Clear();

        if (!tryGetEntriesForPath(info, out var items)) return;

        foreach (var item in items)
        {
            switch (item)
            {
                case DirectoryInfo dir:
                    filesFlow.Add(new DirectoryEntry(dir, this));
                    break;

                case FileInfo file:
                    filesFlow.Add(new FileEntry(file, this));
                    break;
            }
        }

        updateSearch();
    }

    private void showDriveSelect()
    {
        filesFlow.FadeOut(200);
        drivesFlow.Clear();

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady) continue;

            drivesFlow.Add(new DriveEntry(drive, this));
        }

        drivesFlow.FadeInFromZero(200);
    }

    private void onSelect()
    {
        if (ShowFiles)
        {
            if (CurrentFile == null) return;

            Logger.Log($"Selected file {CurrentFile.FullName}", LoggingTarget.Runtime, LogLevel.Debug);
            OnFileSelected?.Invoke(CurrentFile);
        }
        else
        {
            if (currentDirectory == null) return;

            Logger.Log($"Selected path {currentDirectory.FullName}", LoggingTarget.Runtime, LogLevel.Debug);
            OnDirectorySelected?.Invoke(currentDirectory);
        }

        Hide();
    }

    private bool tryGetEntriesForPath(DirectoryInfo info, out List<FileSystemInfo> items)
    {
        items = new List<FileSystemInfo>();

        try
        {
            items.AddRange(info.GetDirectories());

            if (!ShowFiles) return true;

            var files = info.GetFiles();

            if (AllowedExtensions.Length > 0)
                files = files.Where(f => AllowedExtensions.Contains(f.Extension)).ToArray();

            items.AddRange(files);

            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to get entries for {info?.FullName}!");
            errorText.Text = e.Message;
            errorContainer.FadeInFromZero(200);
            errorSample?.Play();
            return false;
        }
    }

    private void updateSearch()
    {
        if (currentDirectory == null) return;

        var search = searchTextBox.Text;

        if (string.IsNullOrEmpty(search))
        {
            foreach (var item in filesFlow)
                item.Show();
        }
        else
        {
            foreach (var item in filesFlow)
            {
                if (item is not GenericEntry entry) continue;

                if (entry.Text.Contains(search, StringComparison.OrdinalIgnoreCase))
                    entry.Show();
                else
                    entry.Hide();
            }
        }

        if (filesFlow.Any(e => e.Alpha > 0))
            noFilesContainer.FadeOut(200);
        else
            noFilesContainer.FadeIn(200);
    }

    public override void Show() => this.ScaleTo(.9f).FadeInFromZero(200).ScaleTo(1f, 1000, Easing.OutElastic);
    public override void Hide() => this.FadeOut(200).ScaleTo(.9f, 400, Easing.OutQuint);

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Hide();
                return true;

            default:
                return false;
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    private partial class PathTextBox : FluXisTextBox
    {
        protected override float LeftRightPadding => 10;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            TextContainer.Height = .5f;
            BackgroundFocused = FluXisColors.Background2;
            BackgroundUnfocused = FluXisColors.Background2;
            BackgroundCommit = FluXisColors.Background2;
            CornerRadius = 10;
        }
    }

    private partial class OneDirUpButton : ClickableContainer
    {
        [Resolved]
        private UISamples samples { get; set; }

        private Box hover { get; set; }
        private Box flash { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(60);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 10;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                hover = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome6.Solid.ChevronLeft,
                    Size = new Vector2(20)
                }
            };
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            this.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }

        protected override bool OnHover(HoverEvent e)
        {
            samples.Hover();
            hover.FadeTo(.2f, 50);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.FadeOut(200);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            flash.FadeOutFromOne(1000, Easing.OutQuint);
            return base.OnClick(e);
        }
    }
}
