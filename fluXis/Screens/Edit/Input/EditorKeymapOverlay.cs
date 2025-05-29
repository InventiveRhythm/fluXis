using System;
using System.Collections.Generic;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Overlay;
using fluXis.Overlay.Settings.UI;
using fluXis.Overlay.Settings.UI.Keybind;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Edit.Input;

public partial class EditorKeymapOverlay : IconEntranceOverlay, IKeyBindingHandler<FluXisGlobalKeybind>
{
    protected override IconUsage Icon => FontAwesome6.Solid.Keyboard;
    protected override ColourInfo BackgroundColor => FluXisColors.Background1;
    protected override float IconRotation => 0;

    private EditorKeybindingContainer bindings { get; }

    private FillFlowContainer flow;

    public EditorKeymapOverlay(EditorKeybindingContainer bindings)
    {
        this.bindings = bindings;
    }

    protected override IEnumerable<Drawable> CreateContent() => new GridContainer
    {
        RelativeSizeAxes = Axes.Both,
        RowDimensions = new Dimension[]
        {
            new(GridSizeMode.Absolute, 80),
            new()
        },
        Content = new[]
        {
            new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Margin = new MarginPadding(28),
                            Spacing = new Vector2(8),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new FluXisSpriteIcon
                                {
                                    Icon = FontAwesome6.Solid.Keyboard,
                                    Size = new Vector2(20),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                },
                                new FluXisSpriteText
                                {
                                    Text = "Editor Keymap",
                                    WebFontSize = 16,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                }
                            }
                        },
                        new IconButton
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Icon = FontAwesome6.Solid.XMark,
                            Margin = new MarginPadding(16),
                            ButtonSize = 48,
                            IconSize = 24,
                            Action = Hide
                        }
                    },
                }
            },
            new Drawable[]
            {
                new FluXisScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarVisible = false,
                    Child = flow = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(48) { Top = 24 },
                        Spacing = new Vector2(12)
                    }
                }
            }
        }
    }.Yield();

    private IEnumerable<Drawable> createContent()
    {
        var map = bindings.Keymap;
        var strings = LocalizationStrings.Settings.Input;

        yield return createScrollActionDropdown(strings.ScrollAction, map.Scroll.Normal, EditorScrollAction.Seek, v => map.Scroll.Normal = v);
        yield return createScrollActionDropdown(strings.ScrollShiftAction, map.Scroll.Shift, EditorScrollAction.Snap, v => map.Scroll.Shift = v);
        yield return createScrollActionDropdown(strings.ScrollCtrlAction, map.Scroll.Control, EditorScrollAction.Zoom, v => map.Scroll.Control = v);
        yield return createScrollActionDropdown(strings.ScrollCtrlShiftAction, map.Scroll.ControlShift, EditorScrollAction.Rate, v => map.Scroll.ControlShift = v);

        yield return createToggle("Invert mouse scrolling", map.InvertScroll, false, v => map.InvertScroll = v);

        foreach (var value in Enum.GetValues<EditorKeybinding>())
        {
            yield return new SettingsCallbackKeybind<EditorKeybinding>(b => bindings.GetBindFor(b))
            {
                BindUpdated = bindings.UpdateBinding,
                Label = value.GetDescription(),
                Keybinds = new[] { value },
                Padded = false
            };
        }
    }

    private Drawable createToggle(LocalisableString label, bool current, bool def, Action<bool> update)
    {
        var bind = new BindableBool()
        {
            Value = current,
            Default = def
        };

        var drop = new SettingsToggle()
        {
            Label = label,
            Bindable = bind
        };

        bind.ValueChanged += v => update?.Invoke(v.NewValue);
        return drop;
    }

    private Drawable createScrollActionDropdown(LocalisableString label, EditorScrollAction current, EditorScrollAction def, Action<EditorScrollAction> update)
    {
        var bind = new Bindable<EditorScrollAction>
        {
            Value = current,
            Default = def
        };

        var drop = new SettingsDropdown<EditorScrollAction>
        {
            Label = label,
            Items = Enum.GetValues<EditorScrollAction>(),
            Bindable = bind
        };

        bind.ValueChanged += v => update?.Invoke(v.NewValue);
        return drop;
    }

    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;
    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnMouseDown(MouseDownEvent e) => true;

    public override void Show()
    {
        flow.ChildrenEnumerable = createContent();
        base.Show();
    }

    public override void Hide()
    {
        bindings.SaveCurrent();
        base.Hide();
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        Hide();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
