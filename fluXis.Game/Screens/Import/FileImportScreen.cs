using System;
using System.IO;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.FileSelect;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Import;

public partial class FileImportScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1.4f;
    public override float ParallaxStrength => 3f;
    public override bool ShowToolbar => false;
    public override float BackgroundBlur => 0.2f;
    public override bool AllowMusicControl => false;

    public Action<FileInfo> OnFileSelected { get; set; }
    public string[] AllowedExtensions { get; set; }

    private FileSelect fileSelect;
    private FluXisSpriteText selectedFileText;
    private FluXisButton importButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(.8f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                fileSelect = new FileSelect(validFileExtensions: AllowedExtensions)
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = .9f,
                    Padding = new MarginPadding { Top = 25, Horizontal = 25 }
                },
                new GridContainer
                {
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize),
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    RowDimensions = new[] { new Dimension() },
                    RelativeSizeAxes = Axes.Both,
                    Height = .1f,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new FluXisButton
                            {
                                Width = 200,
                                Height = 50,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Margin = new MarginPadding { Left = 25 },
                                Text = "Cancel",
                                Action = this.Exit
                            },
                            selectedFileText = new FluXisSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FontSize = 24
                            },
                            importButton = new FluXisButton
                            {
                                Width = 200,
                                Height = 50,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Margin = new MarginPadding { Right = 25 },
                                Text = "Import",
                                Action = () =>
                                {
                                    if (fileSelect.CurrentFile.Value != null)
                                    {
                                        OnFileSelected?.Invoke(fileSelect.CurrentFile.Value);
                                        this.Exit();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        fileSelect.CurrentFile.BindValueChanged(file =>
        {
            selectedFileText.Text = file.NewValue != null ? file.NewValue.Name : "Select a file to import";
            importButton.Enabled = file.NewValue != null;
        }, true);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.ScaleTo(.9f).ScaleTo(1, 300, Easing.OutQuint).FadeIn(200);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.ScaleTo(.9f, 300, Easing.OutQuint).FadeOut(200);
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back) return false;

        this.Exit();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
