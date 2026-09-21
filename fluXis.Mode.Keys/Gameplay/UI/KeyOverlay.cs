using System.Linq;
using fluXis.Database;
using fluXis.Graphics.Sprites.Text;
using fluXis.Modes.Gameplay;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;

namespace fluXis.Mode.Keys.Gameplay.UI;

public partial class KeyOverlay : Container
{
    [Resolved]
    private RulesetContainer ruleset { get; set; } = null!;

    [Resolved]
    private Playfield playfield { get; set; } = null!;

    [Resolved]
    private FluXisRealm realm { get; set; } = null!;

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; } = null!;

    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; } = null!;

    [Resolved]
    private KeysKeybindContainer keybinds { get; set; } = null!;

    private FillFlowContainer flow = null!;
    private int keyCount;
    private bool first = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        var binds = keybinds.Keys;

        if (ruleset.Map.IsDual)
        {
            var half = binds.Length / 2;
            var start = half * playfield.PlayerIndex;
            binds = binds[start..half];
        }

        InternalChild = flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Children =
            [
                .. binds.Select(x => new KeybindContainer(keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(x, realm).KeyCombination)))
            ]
        };
    }

    protected override void Update()
    {
        if (ruleset.AlwaysShowKeys)
            Alpha = 1;
        else if (keyCount != laneSwitchManager.CurrentCount)
        {
            keyCount = laneSwitchManager.CurrentCount;

            const double fade_duration = 300;
            var duration = first ? 8000d : 4000d;
            duration -= fade_duration * 2;
            first = false;

            flow.FadeIn(fade_duration).Delay(duration).FadeOut(fade_duration);
        }

        Y = -laneSwitchManager.HitPosition - 50;

        for (var i = 0; i < flow.Count; i++)
        {
            var con = flow[i];
            con.Width = laneSwitchManager.WidthFor(i + 1);
            con.Alpha = con.Width == 0 ? 0 : 1;
        }
    }

    private partial class KeybindContainer : Container
    {
        public string Keybind { get; init; }

        public KeybindContainer(string keybind)
        {
            Keybind = keybind;
        }

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
