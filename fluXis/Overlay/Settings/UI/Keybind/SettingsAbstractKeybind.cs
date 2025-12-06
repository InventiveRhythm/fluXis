using System;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils.Extensions;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;
using osu.Framework.Logging;

namespace fluXis.Overlay.Settings.UI.Keybind;

public abstract partial class SettingsAbstractKeybind<T> : SettingsItem
    where T : Enum
{
    public override bool AcceptsFocus => true;

    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; }

    public T[] Keybinds { get; init; }

    private FillFlowContainer<KeybindContainer> flow;
    private int index = -1;
    private Sample menuScroll;

    private bool? cachedIsDefault;

    protected override bool IsDefault
    {
        get
        {
            if (cachedIsDefault != null) return cachedIsDefault.Value;

            try
            {
                KeyBinding[] defaultBindings = Keybinds.Select(InputUtils.GetDefaultBindingFor<T>).ToArray();
                KeyBinding[] keybindCombos = Keybinds.Select(GetComboFor).ToArray();

                foreach (var (keybind, defaultBinding) in keybindCombos.Zip(defaultBindings))
                {
                    if (!defaultBinding.KeyCombination.Keys.SequenceEqual(keybind.KeyCombination.Keys))
                    {
                        cachedIsDefault = false;
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed get default bindings for settings");
            }

            cachedIsDefault = true;
            return true;
        }
    }

    protected SettingsAbstractKeybind()
    {
        Padded = true;
        HideWhenDisabled = true;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        menuScroll = samples.Get("UI/scroll");
    }

    protected override Drawable CreateContent() => flow = new FillFlowContainer<KeybindContainer>
    {
        AutoSizeAxes = Axes.Both,
        Anchor = Anchor.CentreRight,
        Origin = Anchor.CentreRight,
        Direction = FillDirection.Horizontal,
        Spacing = new Vector2(6, 0)
    };

    protected override void LoadComplete()
    {
        // we need this here because else the keybinds will be empty on first start
        foreach (var keybind in Keybinds)
        {
            flow.Add(new KeybindContainer(keybind, this)
            {
                KeybindText = keyCombinationProvider.GetReadableString(GetComboFor(keybind).KeyCombination)
            });
        }

        base.UpdateResetButton();
    }

    protected override void Reset()
    {
        foreach (var keybind in Keybinds)
        {
            var defaultBinding = InputUtils.GetDefaultBindingFor(keybind);
            UpdateBinding(keybind, defaultBinding.KeyCombination);
        }

        for (int i = 0; i < flow.Children.Count; i++)
        {
            var keybind = Keybinds[i];
            var bind = GetComboFor(keybind);
            flow[i].KeybindText = keyCombinationProvider.GetReadableString(bind.KeyCombination);
        }

        clearIsDefaultCache();
        index = -1;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        index = 0;
        menuScroll?.Play();
        return true;
    }

    protected override void OnFocusLost(FocusLostEvent e) => index = -1;

    protected override void Update()
    {
        for (var i = 0; i < flow.Children.Count; i++)
        {
            var child = flow[i];
            var isCurrent = i == index;

            if (child.IsCurrent.Value != isCurrent)
                child.IsCurrent.Value = isCurrent;
        }

        base.Update();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat || index == -1 || !HasFocus) return false;

        if (e.Key >= Key.F1)
            updateBinding(KeyCombination.FromInputState(e.CurrentState), KeyCombination.FromKey(e.Key));

        return true;
    }

    protected override void OnKeyUp(KeyUpEvent e)
    {
        if (index == -1 || !HasFocus) return;

        if (e.Key < Key.F1)
            updateBinding(new KeyCombination(KeyCombination.FromKey(e.Key)));
    }

    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        if (index == -1 || !HasFocus) return false;

        updateBinding(KeyCombination.FromInputState(e.CurrentState), KeyCombination.FromJoystickButton(e.Button));
        return true;
    }

    protected override bool OnMidiDown(MidiDownEvent e)
    {
        if (index == -1 || !HasFocus) return false;

        updateBinding(KeyCombination.FromInputState(e.CurrentState), KeyCombination.FromMidiKey(e.Key));
        return true;
    }

    private void updateBinding(KeyCombination combination, InputKey key) => updateBinding(new KeyCombination(combination.Keys.Where(KeyCombination.IsModifierKey).Append(key).ToArray()));

    private void updateBinding(KeyCombination combination)
    {
        if (index < Keybinds.Length)
        {
            var keybind = Keybinds[index];
            UpdateBinding(keybind, combination);

            clearIsDefaultCache();

            var bind = GetComboFor(keybind);
            flow[index].KeybindText = keyCombinationProvider.GetReadableString(bind.KeyCombination);

            index++;
            menuScroll?.Play();
        }
        else index = -1;
    }

    private void clearIsDefaultCache()
    {
        cachedIsDefault = null;
    }

    private void select(T bind)
    {
        index = Array.IndexOf(Keybinds, bind);
        menuScroll?.Play();
    }

    protected abstract KeyBinding GetComboFor(T bind);
    protected abstract void UpdateBinding(T bind, KeyCombination combo);

    private partial class KeybindContainer : Container
    {
        public string KeybindText { set => text.Text = value; }

        public BindableBool IsCurrent { get; } = new();

        private T bind { get; }
        private SettingsAbstractKeybind<T> parent { get; }

        private Box box { get; }
        private FluXisSpriteText text { get; }

        public KeybindContainer(T bind, SettingsAbstractKeybind<T> parent)
        {
            this.bind = bind;
            this.parent = parent;

            AutoSizeAxes = Axes.X;
            Height = 36;
            CornerRadius = 6;
            Masking = true;

            BorderColour = Theme.Highlight;

            Children = new Drawable[]
            {
                box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = .25f
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Padding = new MarginPadding { Horizontal = 12 },
                    Child = text = new FluXisSpriteText
                    {
                        WebFontSize = 16,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };

            IsCurrent.BindValueChanged(updateState, true);
        }

        private void updateState(ValueChangedEvent<bool> e) => this.BorderTo(e.NewValue ? 3 : 0, 300, Easing.OutQuint);

        protected override bool OnClick(ClickEvent e)
        {
            parent.select(bind);
            return true;
        }
    }
}
