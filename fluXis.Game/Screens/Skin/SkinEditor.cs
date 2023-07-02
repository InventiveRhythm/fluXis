using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Skin.UI;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Skin;

public partial class SkinEditor : FluXisScreen
{
    public override float Zoom => 1f;
    public override float ParallaxStrength => 1f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .3f;
    public override bool AllowMusicControl => true;

    [Resolved]
    private SkinManager skinManager { get; set; }

    public Skinning.Json.Skin Skin { get; private set; }
    public Bindable<int> KeyMode { get; private set; } = new(4);

    private SkinEditorPlayfield playfield;

    private SkinEditorTextBox hitPositionTextBox;
    private SkinEditorTextBox columnWidthTextBox;

    [BackgroundDependencyLoader]
    private void load(NotificationOverlay notifications)
    {
        Skin = skinManager.CurrentSkin.Copy();

        KeyMode.BindValueChanged(e =>
        {
            playfield.KeyMode = e.NewValue;
            playfield.Reload();

            hitPositionTextBox.TextBox.Text = Skin.GetKeymode(KeyMode.Value).HitPosition.ToString();
            columnWidthTextBox.TextBox.Text = Skin.GetKeymode(KeyMode.Value).ColumnWidth.ToString();
        });

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10) { Top = 55 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(),
                        new(GridSizeMode.Absolute, 20),
                        new(GridSizeMode.Absolute, 300)
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            playfield = new SkinEditorPlayfield
                            {
                                Skin = Skin,
                                SkinManager = skinManager,
                                KeyMode = KeyMode.Value
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
                                    new FluXisScrollContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding(10),
                                        Child = new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0, 10),
                                            Children = new Drawable[]
                                            {
                                                hitPositionTextBox = new SkinEditorTextBox
                                                {
                                                    Text = "Hit Position",
                                                    DefaultText = Skin.GetKeymode(KeyMode.Value).HitPosition.ToString(),
                                                    OnTextChanged = () =>
                                                    {
                                                        if (int.TryParse(hitPositionTextBox.TextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int y))
                                                            Skin.GetKeymode(KeyMode.Value).HitPosition = y;
                                                        else
                                                            hitPositionTextBox.TextBox.NotifyError();
                                                    }
                                                },
                                                columnWidthTextBox = new SkinEditorTextBox
                                                {
                                                    Text = "Column Width",
                                                    DefaultText = Skin.GetKeymode(KeyMode.Value).ColumnWidth.ToString(),
                                                    OnTextChanged = () =>
                                                    {
                                                        if (int.TryParse(columnWidthTextBox.TextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int w))
                                                            Skin.GetKeymode(KeyMode.Value).ColumnWidth = w;
                                                        else
                                                            columnWidthTextBox.TextBox.NotifyError();
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
            },
            new EditorMenuBar
            {
                Items = new[]
                {
                    new MenuItem("File")
                    {
                        Items = new[]
                        {
                            new MenuItem("Save", () =>
                            {
                                notifications.Post("Skin Saved");
                                skinManager.UpdateAndSave(Skin);
                            }),
                            new MenuItem("Exit", this.Exit)
                        }
                    },
                    new MenuItem("Key Mode")
                    {
                        Items = new[]
                        {
                            new MenuItem("1 Key", () => KeyMode.Value = 1),
                            new MenuItem("2 Keys", () => KeyMode.Value = 2),
                            new MenuItem("3 Keys", () => KeyMode.Value = 3),
                            new MenuItem("4 Keys", () => KeyMode.Value = 4),
                            new MenuItem("5 Keys", () => KeyMode.Value = 5),
                            new MenuItem("6 Keys", () => KeyMode.Value = 6),
                            new MenuItem("7 Keys", () => KeyMode.Value = 7),
                            new MenuItem("8 Keys", () => KeyMode.Value = 8),
                            new MenuItem("9 Keys", () => KeyMode.Value = 9),
                            new MenuItem("10 Keys", () => KeyMode.Value = 10)
                        }
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        skinManager.CanChangeSkin = false;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        skinManager.CanChangeSkin = true;
        return base.OnExiting(e);
    }
}
