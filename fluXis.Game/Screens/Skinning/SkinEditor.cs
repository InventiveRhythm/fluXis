using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Json;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Skinning;

public partial class SkinEditor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>, IKeyBindingHandler<EditorKeybinding>
{
    public override float Zoom => 1f;
    public override float ParallaxStrength => .05f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .3f;
    public override bool AllowMusicControl => true;

    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    private SkinJson skinJson { get; set; }
    private Bindable<int> keyMode { get; } = new(4);

    private EditorMenuBar menuBar;
    private PopoverContainer content;

    private SkinEditorPlayfield playfield;

    private PointSettingsTextBox hitPositionTextBox;
    private PointSettingsTextBox columnWidthTextBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        skinJson = skinManager.SkinJson.DeepClone();

        keyMode.BindValueChanged(e =>
        {
            playfield.KeyMode = e.NewValue;
            playfield.Reload();

            hitPositionTextBox.TextBox.Text = skinJson.GetKeymode(keyMode.Value).HitPosition.ToString();
            columnWidthTextBox.TextBox.Text = skinJson.GetKeymode(keyMode.Value).ColumnWidth.ToString();
        });

        InternalChild = new EditorKeybindingContainer(this, realm)
        {
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = 45 },
                    Child = content = new PopoverContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(.9f),
                        Child = new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new Dimension[]
                            {
                                new(),
                                new(GridSizeMode.Absolute, 480)
                            },
                            Content = new[]
                            {
                                new[]
                                {
                                    playfield = new SkinEditorPlayfield
                                    {
                                        SkinJson = skinJson,
                                        SkinManager = skinManager,
                                        KeyMode = keyMode.Value
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
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
                                                Padding = new MarginPadding(20),
                                                Child = new FillFlowContainer
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    AutoSizeAxes = Axes.Y,
                                                    Direction = FillDirection.Vertical,
                                                    Spacing = new Vector2(20),
                                                    Children = new Drawable[]
                                                    {
                                                        new FluXisSpriteText
                                                        {
                                                            Text = "Keymode-specific",
                                                            WebFontSize = 20
                                                        },
                                                        hitPositionTextBox = new PointSettingsTextBox
                                                        {
                                                            Text = "Hit Position",
                                                            DefaultText = skinJson.GetKeymode(keyMode.Value).HitPosition.ToString(),
                                                            OnTextChanged = box =>
                                                            {
                                                                if (box.Text.TryParseIntInvariant(out var y))
                                                                    skinJson.GetKeymode(keyMode.Value).HitPosition = y;
                                                                else
                                                                    box.NotifyError();
                                                            }
                                                        },
                                                        columnWidthTextBox = new PointSettingsTextBox
                                                        {
                                                            Text = "Column Width",
                                                            DefaultText = skinJson.GetKeymode(keyMode.Value).ColumnWidth.ToString(),
                                                            OnTextChanged = box =>
                                                            {
                                                                if (box.Text.TryParseIntInvariant(out var w))
                                                                    skinJson.GetKeymode(keyMode.Value).ColumnWidth = w;
                                                                else
                                                                    box.NotifyError();
                                                            }
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = "Judgements",
                                                            WebFontSize = 20
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Flawless",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Flawless),
                                                            OnColorChanged = c => skinJson.Judgements.Flawless = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Perfect",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Perfect),
                                                            OnColorChanged = c => skinJson.Judgements.Perfect = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Great",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Great),
                                                            OnColorChanged = c => skinJson.Judgements.Great = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Alright",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Alright),
                                                            OnColorChanged = c => skinJson.Judgements.Alright = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Okay",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Okay),
                                                            OnColorChanged = c => skinJson.Judgements.Okay = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "Miss",
                                                            Color = skinJson.GetColorForJudgement(Judgement.Miss),
                                                            OnColorChanged = c => skinJson.Judgements.Miss = c.ToHex()
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = "Snap Colors",
                                                            WebFontSize = 20
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/1",
                                                            Color = skinJson.SnapColors.GetColor(0),
                                                            OnColorChanged = c => skinJson.SnapColors.Third = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/2",
                                                            Color = skinJson.SnapColors.GetColor(1),
                                                            OnColorChanged = c => skinJson.SnapColors.Fourth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/3",
                                                            Color = skinJson.SnapColors.GetColor(2),
                                                            OnColorChanged = c => skinJson.SnapColors.Sixth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/4",
                                                            Color = skinJson.SnapColors.GetColor(3),
                                                            OnColorChanged = c => skinJson.SnapColors.Eighth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/6",
                                                            Color = skinJson.SnapColors.GetColor(4),
                                                            OnColorChanged = c => skinJson.SnapColors.Twelfth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/8",
                                                            Color = skinJson.SnapColors.GetColor(5),
                                                            OnColorChanged = c => skinJson.SnapColors.Sixteenth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/12",
                                                            Color = skinJson.SnapColors.GetColor(6),
                                                            OnColorChanged = c => skinJson.SnapColors.TwentyFourth = c.ToHex()
                                                        },
                                                        new PointSettingsColor
                                                        {
                                                            Text = "1/16 and unsnapped",
                                                            Color = skinJson.SnapColors.GetColor(7),
                                                            OnColorChanged = c => skinJson.SnapColors.FortyEighth = c.ToHex()
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
                menuBar = new EditorMenuBar
                {
                    Items = new[]
                    {
                        new FluXisMenuItem("File", FontAwesome6.Solid.File)
                        {
                            Items = new[]
                            {
                                new FluXisMenuItem("Save", FontAwesome6.Solid.FloppyDisk, save),
                                new("Open Skin Folder", FontAwesome6.Solid.FolderOpen, skinManager.OpenFolder),
                                new FluXisMenuItem("Exit", FontAwesome6.Solid.DoorOpen, this.Exit)
                            }
                        },
                        new FluXisMenuItem("Key Mode") { Items = getKeymodeItems() }
                    }
                }
            }
        };
    }

    private IReadOnlyList<MenuItem> getKeymodeItems()
    {
        return Enumerable.Range(1, 10)
                         .Select(x => new FluXisMenuItem($"{x} Key{(x > 1 ? "s" : "")}", () => keyMode.Value = x))
                         .ToList();
    }

    private void save()
    {
        notifications.SendText("Skin Saved", "", FontAwesome6.Solid.Check);
        skinManager.UpdateAndSave(skinJson);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        switch (e.Action)
        {
            case EditorKeybinding.Save:
                save();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        skinManager.CanChangeSkin = false;

        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeInFromZero(FADE_DURATION);
            menuBar.MoveToY(-menuBar.Height).MoveToY(0, MOVE_DURATION, Easing.OutQuint);
            content.ScaleTo(1f, MOVE_DURATION, Easing.OutQuint);
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        skinManager.CanChangeSkin = true;

        this.FadeOut(FADE_DURATION);
        menuBar.MoveToY(-menuBar.Height, MOVE_DURATION, Easing.OutQuint);
        content.ScaleTo(.9f, MOVE_DURATION, Easing.OutQuint);

        return base.OnExiting(e);
    }
}
