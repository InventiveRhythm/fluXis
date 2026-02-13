using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Input;
using fluXis.Overlay.Notifications;
using fluXis.Scoring.Enums;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.UI.MenuBar;
using fluXis.Screens.Edit.UI.Variable;
using fluXis.Skinning;
using fluXis.Skinning.Json;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Skinning;

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

    private SkinJson skinJson { get; set; }
    private Bindable<int> keyMode { get; } = new(4);

    private EditorMenuBar menuBar;
    private PopoverContainer content;

    private SkinEditorPlayfield playfield;

    private EditorVariableTextBox hitPositionTextBox;
    private EditorVariableTextBox columnWidthTextBox;
    private EditorVariableToggle receptorsFirstToggle;
    private EditorVariableTextBox receptorsPositionTextBox;

    [BackgroundDependencyLoader]
    private void load(GameHost host, FluXisConfig config)
    {
        skinJson = skinManager.SkinJson.JsonCopy();

        keyMode.BindValueChanged(e =>
        {
            playfield.KeyMode = e.NewValue;
            playfield.Reload();

            var mode = skinJson.GetKeymode(keyMode.Value);

            hitPositionTextBox.TextBox.Text = mode.HitPosition.ToString();
            columnWidthTextBox.TextBox.Text = mode.ColumnWidth.ToString();
            receptorsFirstToggle.Bindable.Value = mode.ReceptorsFirst;
            receptorsPositionTextBox.TextBox.Text = mode.ReceptorOffset.ToString();
        });

        InternalChild = new EditorKeybindingContainer(this, config.GetBindable<string>(FluXisSetting.EditorKeymap), host)
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
                                        Skin = skinManager,
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
                                                Colour = Theme.Background2
                                            },
                                            new FluXisScrollContainer
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Padding = new MarginPadding(20),
                                                ScrollbarVisible = false,
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
                                                        hitPositionTextBox = new EditorVariableTextBox
                                                        {
                                                            Text = "Hit Position",
                                                            CurrentValue = skinJson.GetKeymode(keyMode.Value).HitPosition.ToString(),
                                                            OnValueChanged = box =>
                                                            {
                                                                if (box.Text.TryParseIntInvariant(out var y))
                                                                    skinJson.GetKeymode(keyMode.Value).HitPosition = y;
                                                                else
                                                                    box.NotifyError();
                                                            }
                                                        },
                                                        columnWidthTextBox = new EditorVariableTextBox
                                                        {
                                                            Text = "Column Width",
                                                            CurrentValue = skinJson.GetKeymode(keyMode.Value).ColumnWidth.ToString(),
                                                            OnValueChanged = box =>
                                                            {
                                                                if (box.Text.TryParseIntInvariant(out var w))
                                                                    skinJson.GetKeymode(keyMode.Value).ColumnWidth = w;
                                                                else
                                                                    box.NotifyError();
                                                            }
                                                        },
                                                        receptorsFirstToggle = new EditorVariableToggle
                                                        {
                                                            Text = "Receptors First",
                                                            TooltipText = "Layers the receptors first. (Behind the notes)",
                                                            CurrentValue = skinJson.GetKeymode(keyMode.Value).ReceptorsFirst,
                                                            OnValueChanged = v => skinJson.GetKeymode(keyMode.Value).ReceptorsFirst = v
                                                        },
                                                        receptorsPositionTextBox = new EditorVariableTextBox
                                                        {
                                                            Text = "Receptor Offset",
                                                            CurrentValue = skinJson.GetKeymode(keyMode.Value).ReceptorOffset.ToString(),
                                                            OnValueChanged = box =>
                                                            {
                                                                if (box.Text.TryParseIntInvariant(out var w))
                                                                    skinJson.GetKeymode(keyMode.Value).ReceptorOffset = w;
                                                                else
                                                                    box.NotifyError();
                                                            }
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = "Judgements",
                                                            WebFontSize = 20
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Flawless",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Flawless),
                                                            OnValueChanged = c => skinJson.Judgements.Flawless = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Perfect",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Perfect),
                                                            OnValueChanged = c => skinJson.Judgements.Perfect = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Great",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Great),
                                                            OnValueChanged = c => skinJson.Judgements.Great = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Alright",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Alright),
                                                            OnValueChanged = c => skinJson.Judgements.Alright = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Okay",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Okay),
                                                            OnValueChanged = c => skinJson.Judgements.Okay = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "Miss",
                                                            CurrentValue = skinJson.GetColorForJudgement(Judgement.Miss),
                                                            OnValueChanged = c => skinJson.Judgements.Miss = c.ToHex()
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = "Snap Colors",
                                                            WebFontSize = 20
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/1",
                                                            CurrentValue = skinJson.SnapColors.GetColor(0),
                                                            OnValueChanged = c => skinJson.SnapColors.Third = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/2",
                                                            CurrentValue = skinJson.SnapColors.GetColor(1),
                                                            OnValueChanged = c => skinJson.SnapColors.Fourth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/3",
                                                            CurrentValue = skinJson.SnapColors.GetColor(2),
                                                            OnValueChanged = c => skinJson.SnapColors.Sixth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/4",
                                                            CurrentValue = skinJson.SnapColors.GetColor(3),
                                                            OnValueChanged = c => skinJson.SnapColors.Eighth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/6",
                                                            CurrentValue = skinJson.SnapColors.GetColor(4),
                                                            OnValueChanged = c => skinJson.SnapColors.Twelfth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/8",
                                                            CurrentValue = skinJson.SnapColors.GetColor(5),
                                                            OnValueChanged = c => skinJson.SnapColors.Sixteenth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/12",
                                                            CurrentValue = skinJson.SnapColors.GetColor(6),
                                                            OnValueChanged = c => skinJson.SnapColors.TwentyFourth = c.ToHex()
                                                        },
                                                        new EditorVariableColor
                                                        {
                                                            Text = "1/16 and unsnapped",
                                                            CurrentValue = skinJson.SnapColors.GetColor(7),
                                                            OnValueChanged = c => skinJson.SnapColors.FortyEighth = c.ToHex()
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
                        new MenuExpandItem("File", FontAwesome6.Solid.File, new FluXisMenuItem[]
                        {
                            new MenuActionItem("Save", FontAwesome6.Solid.FloppyDisk, save),
                            new MenuActionItem("Open Skin Folder", FontAwesome6.Solid.FolderOpen, skinManager.OpenFolder),
                            new MenuActionItem("Exit", FontAwesome6.Solid.DoorOpen, this.Exit)
                        }),
                        new MenuExpandItem("Key Mode", FontAwesome6.Solid.Keyboard, getKeymodeItems())
                    }
                }
            }
        };
    }

    private IReadOnlyList<MenuItem> getKeymodeItems()
    {
        return Enumerable.Range(1, 10)
                         .Select(x => new MenuActionItem($"{x} Key{(x > 1 ? "s" : "")}", FontAwesome6.Solid.Circle, () => keyMode.Value = x))
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

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            this.FadeInFromZero(Styling.TRANSITION_FADE);
            menuBar.MoveToY(-menuBar.Height).MoveToY(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
            content.ScaleTo(1f, Styling.TRANSITION_MOVE, Easing.OutQuint);
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        skinManager.CanChangeSkin = true;

        this.FadeOut(Styling.TRANSITION_FADE);
        menuBar.MoveToY(-menuBar.Height, Styling.TRANSITION_MOVE, Easing.OutQuint);
        content.ScaleTo(.9f, Styling.TRANSITION_MOVE, Easing.OutQuint);

        return base.OnExiting(e);
    }
}
