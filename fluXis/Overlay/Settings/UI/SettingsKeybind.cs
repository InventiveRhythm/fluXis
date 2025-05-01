using System.Linq;
using fluXis.Database;
using fluXis.Database.Input;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsKeybind : SettingsItem
{
    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; }

    private FluXisRealm realm { get; set; }

    public override bool AcceptsFocus => true;

    public object[] Keybinds;

    private FillFlowContainer<KeybindContainer> flow;

    private int index = -1;

    public SettingsKeybind()
    {
        Padded = true;
        HideWhenDisabled = true;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm)
    {
        this.realm = realm;

        Add(flow = new FillFlowContainer<KeybindContainer>
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(5, 0)
        });
    }

    protected override void LoadComplete()
    {
        // we need this here because else the keybinds will be empty on first start
        foreach (var keybind in Keybinds)
        {
            flow.Add(new KeybindContainer
            {
                Keybind = keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(keybind.ToString(), realm).KeyCombination),
                SettingsKeybind = this
            });
        }
    }

    protected override void Reset()
    {
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            index = 0;
            return true;
        }

        return false;
    }

    protected override void OnFocusLost(FocusLostEvent e) => index = -1;

    protected override void Update()
    {
        for (var i = 0; i < flow.Children.Count; i++)
        {
            var child = flow.Children.ElementAt(i);
            bool isCurrent = i == index;

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

            realm.RunWrite(r =>
            {
                var bind = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());

                if (bind == null)
                {
                    bind = new RealmKeybind
                    {
                        Action = keybind.ToString(),
                        Key = combination.ToString()
                    };

                    r.Add(bind);
                }
                else bind.Key = combination.ToString();
            });

            var bind = InputUtils.GetBindingFor(keybind.ToString(), realm);
            flow.Children.ElementAt(index).Keybind = keyCombinationProvider.GetReadableString(bind.KeyCombination);
            index++;
        }
        else index = -1;
    }

    private partial class KeybindContainer : Container
    {
        public string Keybind { set => text.Text = value; }
        public SettingsKeybind SettingsKeybind { get; init; }

        public BindableBool IsCurrent { get; } = new();

        private readonly FluXisSpriteText text;
        private readonly Box box;

        public KeybindContainer()
        {
            Height = 32;
            AutoSizeAxes = Axes.X;
            CornerRadius = 8;
            Masking = true;

            Children = new Drawable[]
            {
                box = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = 0.2f
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Padding = new MarginPadding { Horizontal = 10 },
                    Child = text = new FluXisSpriteText
                    {
                        FontSize = 24,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };

            IsCurrent.BindValueChanged(updateState, true);
        }

        private void updateState(ValueChangedEvent<bool> e) => box.FadeColour(e.NewValue ? FluXisColors.Text : Colour4.Black, 200);

        protected override bool OnClick(ClickEvent e)
        {
            SettingsKeybind.index = SettingsKeybind.flow.Children.ToList().IndexOf(this);
            return base.OnClick(e);
        }
    }
}
