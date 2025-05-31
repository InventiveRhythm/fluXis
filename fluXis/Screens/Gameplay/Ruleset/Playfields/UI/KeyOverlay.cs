using System.Linq;
using fluXis.Database;
using fluXis.Graphics.Sprites.Text;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;

namespace fluXis.Screens.Gameplay.Ruleset.Playfields.UI;

public partial class KeyOverlay : Container
{
    [Resolved]
    private RulesetContainer ruleset { get; set; }

    [Resolved]
    private Playfield playfield { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; }

    private FillFlowContainer flow;
    private int keyCount;
    private bool first = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        var binds = ruleset.Input.Keys;

        if (ruleset.Input.Dual)
        {
            var half = ruleset.Input.Keys.Count / 2;
            var start = half * playfield.Index;
            binds = binds.GetRange(start, half);
        }

        InternalChild = flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Children = binds.Select(x => new KeybindContainer
            {
                Keybind = keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(x, realm).KeyCombination)
            }).ToList()
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
