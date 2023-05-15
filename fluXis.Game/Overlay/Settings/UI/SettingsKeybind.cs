using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
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

    public int Index = -1;

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm, FluXisKeybindContainer container)
    {
        this.realm = realm;
        this.container = container;

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
                Keybind = getBind(keybind).ToString(),
                SettingsKeybind = this
            });
        }
    }

    private InputKey getBind(FluXisKeybind keybind)
    {
        InputKey key = InputKey.None;

        realm.Run(r =>
        {
            var bind = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());
            if (bind == null) return;

            key = (InputKey)Enum.Parse(typeof(InputKey), bind.Key);
        });

        return key;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            Index = 0;
            return true;
        }

        return false;
    }

    protected override void OnFocusLost(FocusLostEvent e) => Index = -1;

    protected override void Update()
    {
        for (var i = 0; i < flow.Children.Count; i++)
        {
            var child = flow.Children.ElementAt(i);
            bool isCurrent = i == Index;

            if (child.IsCurrent.Value != isCurrent)
                child.IsCurrent.Value = isCurrent;
        }

        base.Update();
    }

    // WHY IS THERE 2 DIFFERENT KEY SYSTEMS
    private InputKey fromEvent(Key key)
    {
        switch (key)
        {
            case Key.ControlLeft:
            case Key.ControlRight:
                return InputKey.Control;

            case Key.ShiftLeft:
            case Key.ShiftRight:
                return InputKey.Shift;

            case Key.AltLeft:
            case Key.AltRight:
                return InputKey.Alt;

            case Key.WinLeft:
            case Key.WinRight:
                return InputKey.Super;

            default:
                try
                {
                    return (InputKey)Enum.Parse(typeof(InputKey), key.ToString());
                }
                catch
                {
                    return InputKey.None;
                }
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat || Index == -1 || !HasFocus) return false;

        if (Index < Keybinds.Length)
        {
            var keybind = Keybinds[Index];
            var iKey = fromEvent(e.Key);

            if (iKey == InputKey.Escape)
            {
                Index = -1;
                return false;
            }

            realm.RunWrite(r =>
            {
                var bind = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());

                if (bind == null)
                {
                    bind = new RealmKeybind
                    {
                        Action = keybind.ToString(),
                        Key = iKey.ToString()
                    };

                    r.Add(bind);
                }
                else bind.Key = iKey.ToString();
            });

            container.Reload();

            flow.Children.ElementAt(Index).Keybind = iKey.ToString();
            Index++;
        }
        else Index = -1;

        return true;
    }

    private partial class KeybindContainer : Container
    {
        public string Keybind { set => text.Text = value; }
        public SettingsKeybind SettingsKeybind { get; set; }

        public BindableBool IsCurrent { get; } = new();

        private readonly SpriteText text;
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
                    Child = text = new SpriteText
                    {
                        Font = FluXisFont.Default(24),
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
            SettingsKeybind.Index = SettingsKeybind.flow.Children.ToList().IndexOf(this);
            return base.OnClick(e);
        }
    }
}
