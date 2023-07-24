using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class KeyOverlay : Container
{
    public GameplayScreen Screen { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    public FluXisKeybind[] Keybinds => Screen.Input.Keys;

    private FillFlowContainer flow;
    private int keyCount;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        InternalChild = flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Children = Keybinds.Select(x => new KeybindContainer
            {
                Keybind = InputUtils.GetReadableString(getBind(x))
            }).ToList()
        };
    }

    protected override void Update()
    {
        if (keyCount != Screen.Playfield.Manager.CurrentKeyCount)
        {
            keyCount = Screen.Playfield.Manager.CurrentKeyCount;
            flow.FadeIn(200).Delay(1200).FadeOut(200);
        }

        Y = -skinManager.CurrentSkin.GetKeymode(keyCount).HitPosition - 50;

        for (var i = 0; i < flow.Count; i++)
        {
            var con = flow[i];
            con.Width = Screen.Playfield.Receptors[i].Width;
            con.Alpha = con.Width == 0 ? 0 : 1;
        }
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

    private partial class KeybindContainer : Container
    {
        public string Keybind { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            AutoSizeAxes = Axes.Y;

            Add(new FluXisSpriteText
            {
                Text = Keybind,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                FontSize = 36
            });
        }
    }
}
