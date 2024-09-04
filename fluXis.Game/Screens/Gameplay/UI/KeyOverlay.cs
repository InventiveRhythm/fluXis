using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class KeyOverlay : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; }

    public List<FluXisGameplayKeybind> Keybinds => screen.Input.Keys;

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
                Keybind = keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(x, realm).KeyCombination)
            }).ToList()
        };
    }

    protected override void Update()
    {
        if (keyCount != laneSwitchManager.CurrentCount)
        {
            keyCount = laneSwitchManager.CurrentCount;
            flow.FadeIn(200).Delay(1200).FadeOut(200);
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
