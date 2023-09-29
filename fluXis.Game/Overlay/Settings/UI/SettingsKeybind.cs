using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsKeybind : SettingsItem
{
    private FluXisRealm realm { get; set; }

    public override bool AcceptsFocus => true;

    public FluXisKeybind[] Keybinds;

    private FillFlowContainer<KeybindContainer> flow;
    private FluXisKeybindContainer container;

    private int index = -1;

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm, FluXisKeybindContainer container)
    {
        this.realm = realm;
        this.container = container;

        TextFlow.Padding = new MarginPadding { Left = 20 };

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
                Keybind = InputUtils.GetReadableCombination(getBind(keybind)),
                SettingsKeybind = this
            });
        }
    }

    public override void Reset()
    {
    }

    private KeyCombination getBind(FluXisKeybind keybind)
    {
        KeyCombination combo = new KeyCombination();

        realm.Run(r =>
        {
            var binding = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());
            if (binding == null) return;

            var split = binding.Key.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length > 1)
            {
                combo = new KeyCombination(split.Select(x => (InputKey)Enum.Parse(typeof(InputKey), x)).ToArray());
                return;
            }

            combo = new KeyCombination((InputKey)Enum.Parse(typeof(InputKey), binding.Key));
        });

        return combo;
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

    private void updateBinding(KeyCombination combination, InputKey key) => updateBinding(new KeyCombination(combination.Keys.Where(KeyCombination.IsModifierKey).Append(key)));

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

            container.Reload();

            flow.Children.ElementAt(index).Keybind = InputUtils.GetReadableCombination(combination);
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

        private void updateState(ValueChangedEvent<bool> e) => box.FadeColour(e.NewValue ? Colour4.White : Colour4.Black, 200);

        protected override bool OnClick(ClickEvent e)
        {
            SettingsKeybind.index = SettingsKeybind.flow.Children.ToList().IndexOf(this);
            return base.OnClick(e);
        }
    }
}
