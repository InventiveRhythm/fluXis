using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using fluXis.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsKeybind : SettingsItem
{
    private FluXisRealm realm { get; set; }

    private readonly FillFlowContainer<SpriteText> flow;
    private readonly FluXisKeybind[] keybinds;
    private FluXisKeybindContainer container;

    private int index;
    private bool isListening;

    public SettingsKeybind(string label, FluXisKeybind[] keybinds)
    {
        this.keybinds = keybinds;

        Add(new SpriteText
        {
            Text = label,
            Font = new FontUsage("Quicksand", 24, "Bold"),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Y = -2
        });

        Add(flow = new FillFlowContainer<SpriteText>
        {
            RelativeSizeAxes = Axes.Y,
            AutoSizeAxes = Axes.X,
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(5, 0),
            Y = -2
        });
    }

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm, FluXisKeybindContainer container)
    {
        this.realm = realm;
        this.container = container;

        foreach (var keybind in keybinds)
        {
            flow.Add(new SpriteText
            {
                Text = getBind(keybind).ToString(),
                Font = new FontUsage("Quicksand", 24, "Bold")
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

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            index = 0;
            isListening = true;
            return true;
        }

        return base.OnMouseDown(e);
    }

    protected override void OnFocusLost(FocusLostEvent e)
    {
        isListening = false;
        base.OnFocusLost(e);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat || !isListening) return false;

        if (index < keybinds.Length)
        {
            var keybind = keybinds[index];
            var key = e.Key;

            if (key == Key.Escape)
            {
                index++;
                return base.OnKeyDown(e);
            }

            realm.RunWrite(r =>
            {
                var bind = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());

                if (bind == null)
                {
                    bind = new RealmKeybind
                    {
                        Action = keybind.ToString(),
                        Key = key.ToString()
                    };

                    r.Add(bind);
                }
                else bind.Key = key.ToString();
            });

            container.Reload();

            flow.Children.ElementAt(index).Text = key.ToString();
            index++;
        }

        return base.OnKeyDown(e);
    }
}
